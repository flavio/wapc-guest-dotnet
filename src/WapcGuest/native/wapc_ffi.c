#include <mono-wasi/driver.h>
#include <assert.h>
#include <stdlib.h>
#include <stdbool.h>

__attribute__((__import_module__("wapc"), __import_name__("__guest_request"))) extern void
__guest_request(char *operation, char *payload);

__attribute__((__import_module__("wapc"), __import_name__("__guest_response"))) extern void
__guest_response(const char *payload, size_t len);

__attribute__((__import_module__("wapc"), __import_name__("__guest_error"))) extern void
__guest_error(const char *payload, size_t len);

__attribute__((__import_module__("wapc"), __import_name__("__host_call"))) extern bool
__host_call(
    const char *binding_payload, size_t binding_len,
    const char *namespace_payload, size_t namespace_len,
    const char *operation_payload, size_t operation_len,
    const char *payload, size_t payload_len);

__attribute__((__import_module__("wapc"), __import_name__("__host_response_len"))) extern size_t
__host_response_len(void);

__attribute__((__import_module__("wapc"), __import_name__("__host_response"))) extern void
__host_response(char *payload);

__attribute__((__import_module__("wapc"), __import_name__("__host_error_len"))) extern size_t
__host_error_len(void);

__attribute__((__import_module__("wapc"), __import_name__("__host_error"))) extern void
__host_error(char *payload);

__attribute__((__import_module__("wapc"), __import_name__("__console_log"))) extern void
__console_log(const char *payload, size_t payload_len);

MonoMethod *method_HandleGuestCall;
MonoObject *interop_instance = 0;

__attribute__((export_name("__guest_call"))) bool __guest_call(size_t operation_len, size_t payload_len)
{
  if (!method_HandleGuestCall)
  {
    method_HandleGuestCall = lookup_dotnet_method("WapcGuest.dll", "WapcGuest", "Interop", "HandleGuestCall", -1);
    assert(method_HandleGuestCall);
  }

  void *method_params[] = {interop_instance, &operation_len, &payload_len};
  MonoObject *exception;
  MonoObject *result = mono_wasm_invoke_method(method_HandleGuestCall, NULL, method_params, &exception);
  assert(!exception);

  bool bool_result = *(bool *)mono_object_unbox(result);
  return bool_result;
}

void set_interop(MonoObject *interop)
{
  interop_instance = interop;
}

void attach_internal_calls()
{
  mono_add_internal_call("WapcGuest.Interop::GuestRequest", __guest_request);
  mono_add_internal_call("WapcGuest.Interop::GuestResponse", __guest_response);
  mono_add_internal_call("WapcGuest.Interop::GuestError", __guest_error);
  mono_add_internal_call("WapcGuest.Interop::HostCall", __host_call);
  mono_add_internal_call("WapcGuest.Interop::HostResponseLen", __host_response_len);
  mono_add_internal_call("WapcGuest.Interop::HostResponse", __host_response);
  mono_add_internal_call("WapcGuest.Interop::HostErrorLen", __host_error_len);
  mono_add_internal_call("WapcGuest.Interop::HostError", __host_error);
  mono_add_internal_call("WapcGuest.Interop::ConsoleLog", __console_log);
  mono_add_internal_call("WapcGuest.Interop::SetInterop", set_interop);
}
