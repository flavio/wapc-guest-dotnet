using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Text;

namespace WapcGuest;

internal class Interop
{
  private Dictionary<string, WapcFunc> Functions;

  public Interop()
  {
    Functions = new Dictionary<string, WapcFunc>();
  }

  public void RegisterFunction(string name, WapcFunc fn)
  {
    Functions[name] = fn;
  }

  public unsafe bool HandleGuestCall(Interop interop, int operationSize, int payloadSize)
  {
    Encoding utf8 = Encoding.UTF8;

    var operationBuf = new byte[operationSize];
    Span<byte> operationBytesSpan = operationBuf;
    var payloadBuf = new byte[payloadSize];
    Span<byte> payloadBytesSpan = payloadBuf;

    fixed (byte* operationBytesPtr = operationBytesSpan, payloadBytesPtr = payloadBytesSpan)
    {
      Interop.GuestRequest(operationBytesPtr, payloadBytesPtr);
    }

    string operation = utf8.GetString(operationBuf);

    if (interop.Functions.ContainsKey(operation))
    {
      byte[] result = interop.Functions[operation](payloadBuf);
      Span<byte> resultBytesSpan = result;
      fixed (byte* resultBytesPtr = resultBytesSpan)
      {
        Interop.GuestResponse(resultBytesPtr, result.Length);
      }

      return true;
    }

    string error = $"Unknown function '{operation}'";
    byte[] errorBuf = utf8.GetBytes(error);
    Span<byte> errorBytesSpan = errorBuf;
    fixed (byte* errorBytesPtr = errorBytesSpan)
    {
      Interop.GuestError(errorBytesPtr, errorBuf.Length);
    }

    return false;
  }

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static extern void SetInterop(Interop owner);

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern void GuestRequest(byte* operation, byte* payload);

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern void GuestResponse(byte* payload, int len);

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern void GuestError(byte* payload, int len);

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern bool HostCall(
    byte* bindingPayload, int bindingLen,
    byte* namespacePayload, int namespaceLen,
    byte* operationPayload, int operationLen,
    byte* payload, int payloadLen);

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern int HostResponseLen();

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern void HostResponse(byte* payload);

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern int HostErrorLen();

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern void HostError(byte* payload);

  [MethodImpl(MethodImplOptions.InternalCall)]
  public static unsafe extern void ConsoleLog(byte* payload, int len);
}
