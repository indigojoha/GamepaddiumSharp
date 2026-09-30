using GamepaddiumSharp.mappings;

namespace GamepaddiumSharp.core;

public sealed class ControllerDescriptor
{
    public required string DisplayName { get; init; }
    public int VendorId { get; init; } = -1;
    public int ProductId { get; init; } = -1;
    public ConnectionType Connection { get; init; } = ConnectionType.Unknown;
    public required Func<IHidConnection, IMappings> MappingsFactory { get; init; }
    public required Func<IHidConnection, IOutputs> OutputsFactory { get; init; }

    public bool Matches(int vendorId, int productId, ConnectionType connection)
    {
        if (VendorId != -1 && VendorId != vendorId) return false;
        if (ProductId != -1 && ProductId != productId) return false;
        if (Connection != ConnectionType.Unknown && Connection != connection) return false;
        return true;
    }
}

public static class ControllerDescriptorRegister
{
    public static List<ControllerDescriptor> List { get; set; } =
    [
        new()
        {
            DisplayName = "DualSense (USB)",
            VendorId = 0x054C, // Sony
            ProductId = 0x0CE6,
            Connection = ConnectionType.Usb,
            MappingsFactory = conn => new DualSenseUsbMappings(),
            OutputsFactory = conn => new DualSenseUsbOutputs(),
        },
        new()
        {
            DisplayName = "DualSense (BT)",
            VendorId = 0x054C,
            ProductId = 0x0CE6,
            Connection = ConnectionType.Bluetooth,
            MappingsFactory = conn => new DualSenseBtMappings(),
            OutputsFactory = conn => new DualSenseBtOutputs(),
        },
        new()
        {
            DisplayName = "Mayflash GameCube Controller Adapter",
            VendorId = 0x0079, // DragonRise Inc. (used by Mayflash)
            ProductId = 0x1844, // GameCube Controller Adapter
            Connection = ConnectionType.Usb,
            MappingsFactory = conn => new GameCubeAdaptedMappings(),
            OutputsFactory = conn => new GameCubeAdaptedOutputs(),
        },

        new()
        {
            DisplayName = "Generic HID Controller",
            VendorId = -1,
            ProductId = -1,
            MappingsFactory = conn => {
                var device = HidSharpConnection.GetHidDevice(conn);
                if (device == null)
                    throw new InvalidOperationException("Failed to get HidDevice from connection for HID descriptor parsing");
                return GenericHidMappings.CreateFromDevice(device);
            },
            OutputsFactory = conn => {
                var device = HidSharpConnection.GetHidDevice(conn);
                if (device == null)
                    throw new InvalidOperationException("Failed to get HidDevice from connection for HID output parsing");
                return GenericHidOutputs.CreateFromDevice(device);
            },
        },
    ];

    public static ControllerDescriptor CreateGenericHidDescriptor(int vendorId, int productId, string? displayName = null)
    {
        return new ControllerDescriptor
        {
            DisplayName = displayName ?? $"Generic HID Controller (VID: 0x{vendorId:X4}, PID: 0x{productId:X4})",
            VendorId = vendorId,
            ProductId = productId,
            MappingsFactory = conn => {
                var device = HidSharpConnection.GetHidDevice(conn);
                if (device == null)
                    throw new InvalidOperationException("Failed to get HidDevice from connection for HID descriptor parsing");
                return GenericHidMappings.CreateFromDevice(device);
            },
            OutputsFactory = conn => {
                var device = HidSharpConnection.GetHidDevice(conn);
                if (device == null)
                    throw new InvalidOperationException("Failed to get HidDevice from connection for HID output parsing");
                return GenericHidOutputs.CreateFromDevice(device);
            },
        };
    }

    public static void AddGenericHidDescriptor(int vendorId, int productId, string? displayName = null)
    {
        List.Insert(List.Count - 1, CreateGenericHidDescriptor(vendorId, productId, displayName));
    }
}
