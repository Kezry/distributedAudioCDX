#include <ntddk.h>

DRIVER_UNLOAD DacdxUnload;
DRIVER_INITIALIZE DriverEntry;

static const WCHAR DeviceNameBuffer[] = L"\\Device\\DacdxVirtualAudio";
static const WCHAR DosDeviceNameBuffer[] = L"\\DosDevices\\DacdxVirtualAudio";

void DacdxUnload(_In_ PDRIVER_OBJECT DriverObject)
{
    UNICODE_STRING dosDeviceName;
    RtlInitUnicodeString(&dosDeviceName, DosDeviceNameBuffer);
    IoDeleteSymbolicLink(&dosDeviceName);
    if (DriverObject->DeviceObject != NULL)
    {
        IoDeleteDevice(DriverObject->DeviceObject);
    }
}

NTSTATUS DriverEntry(_In_ PDRIVER_OBJECT DriverObject, _In_ PUNICODE_STRING RegistryPath)
{
    UNREFERENCED_PARAMETER(RegistryPath);

    UNICODE_STRING deviceName;
    UNICODE_STRING dosDeviceName;
    PDEVICE_OBJECT deviceObject = NULL;

    RtlInitUnicodeString(&deviceName, DeviceNameBuffer);
    NTSTATUS status = IoCreateDevice(
        DriverObject,
        0,
        &deviceName,
        FILE_DEVICE_UNKNOWN,
        FILE_DEVICE_SECURE_OPEN,
        FALSE,
        &deviceObject);

    if (!NT_SUCCESS(status))
    {
        return status;
    }

    RtlInitUnicodeString(&dosDeviceName, DosDeviceNameBuffer);
    status = IoCreateSymbolicLink(&dosDeviceName, &deviceName);
    if (!NT_SUCCESS(status))
    {
        IoDeleteDevice(deviceObject);
        return status;
    }

    DriverObject->DriverUnload = DacdxUnload;
    return STATUS_SUCCESS;
}

