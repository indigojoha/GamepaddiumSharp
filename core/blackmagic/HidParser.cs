using HidSharp;
using HidSharp.Reports;
using GamepaddiumSharp.mappings;

namespace GamepaddiumSharp.core.blackmagic;

public static class HidParser
{
    private readonly record struct RawField(byte ReportId, int UsagePage, int UsageId, Atom Atom, bool IsHat, DataItem Source);

    private static List<RawField> Flatten(ReportDescriptor desc)
    {
        var fields = new List<RawField>();

        foreach (DeviceItem dev in desc.DeviceItems)
        {
            foreach (Report report in dev.Reports.Where(r => r.ReportType == ReportType.Input))
            {
                int declaredDataBits = report.DataItems.Sum(d => d.TotalBits);
                int bitCursor = (report.Length * 8) - declaredDataBits;
                byte reportId = (byte)report.ReportID;

                foreach (DataItem item in report.DataItems)
                {
                    int elementBits = item.ElementBits;

                    for (int i = 0; i < item.ElementCount; i++)
                    {
                        uint packed = 0;
                        foreach (uint v in item.Usages.GetValuesFromIndex(i))
                        {
                            packed = v;
                            break;
                        }

                        int usagePage = (int)(packed >> 16);
                        int usageId = (int)(packed & 0xFFFF);

                        if (!item.IsConstant)
                        {
                            var atom = new Atom
                            {
                                ByteOffset = bitCursor / 8,
                                BitOffset = bitCursor % 8,
                                BitWidth = elementBits,
                                Signed = item.IsLogicalSigned,
                            };

                            fields.Add(new RawField(reportId, usagePage, usageId, atom,
                                IsHat: usagePage == 0x01 && usageId == 0x39, item));
                        }

                        bitCursor += elementBits;
                    }
                }
            }
        }

        return fields;
    }

    private static void ClassifyAxis(RawField f, StumpMappings map)
    {
        if (f.UsagePage != 0x01) return;

        Atom centeredAxis = f.Atom with { AnalogType = AtomIntDouble.NZP };
        Atom zpAxis = f.Atom with { AnalogType = AtomIntDouble.ZP };

        switch (f.UsageId)
        {
            case 0x30: map.LeftStickX = centeredAxis; break;
            case 0x31: map.LeftStickY = centeredAxis; break;
            case 0x32: map.RightStickX = centeredAxis; break;
            case 0x35: map.RightStickY = centeredAxis; break;
            case 0x33: map.LT = zpAxis; break;
            case 0x34: map.RT = zpAxis; break;
        }
    }

    private static void ClassifyHat(RawField f, StumpMappings map)
    {
        if (!f.IsHat) return;
        var hat = f.Atom;

        map.DpadUp = (BoolExpr)(r => { var v = hat.Read(r); return v == 0 || v == 1 || v == 7; });
        map.DpadRight = (BoolExpr)(r => { var v = hat.Read(r); return v is >= 1 and <= 3; });
        map.DpadDown = (BoolExpr)(r => { var v = hat.Read(r); return v is >= 3 and <= 5; });
        map.DpadLeft = (BoolExpr)(r => { var v = hat.Read(r); return v is >= 5 and <= 7; });
    }

    private static void ClassifyButtons(IEnumerable<RawField> buttonFields, StumpMappings map)
    {
        var ordered = buttonFields
            .Where(f => f.UsagePage == 0x09)
            .OrderBy(f => f.UsageId)
            .ToList();

        Mapping[] slots = { map.A, map.B, map.X, map.Y };
        for (int i = 0; i < Math.Min(4, ordered.Count); i++)
        {
            Mapping m = (AtomMapping)(Mapping)ordered[i].Atom;
            switch (i) { case 0: map.A = m; break; case 1: map.B = m; break; case 2: map.X = m; break; case 3: map.Y = m; break; }
        }

        if (ordered.Count > 4) map.LB = (AtomMapping)(Mapping)ordered[4].Atom;
        if (ordered.Count > 5) map.RB = (AtomMapping)(Mapping)ordered[5].Atom;
        if (ordered.Count > 6) map.Select = (AtomMapping)(Mapping)ordered[6].Atom;
        if (ordered.Count > 7) map.Start = (AtomMapping)(Mapping)ordered[7].Atom;
        if (ordered.Count > 8) map.L3 = (AtomMapping)(Mapping)ordered[8].Atom;
        if (ordered.Count > 9) map.R3 = (AtomMapping)(Mapping)ordered[9].Atom;
    }

    public static (StumpMappings, StumpOutputs) CompileFrom(HidDevice dev)
    {
        ReportDescriptor desc = dev.GetReportDescriptor();
        var fields = Flatten(desc);

        StumpMappings map = new();
        StumpOutputs outs = new();

        foreach (var f in fields.Where(f => f.IsHat)) ClassifyHat(f, map);
        foreach (var f in fields.Where(f => !f.IsHat && f.UsagePage == 0x01)) ClassifyAxis(f, map);
        ClassifyButtons(fields, map);

        return (map, outs);
    }
}
