﻿using LocalUtilities.SimpleScript.Serialization;

namespace WarringStates;

public class GridData : ISsSerializable
{
    /// <summary>
    /// 格元边框绘制用笔
    /// </summary>
    public Pen CellPen { get; } = new(Color.FromArgb(200, Color.LightGray), 5f);
    /// <summary>
    /// 节点边框绘制用笔（直线）
    /// </summary>
    public Pen NodePenLine { get; } = new(Color.FromArgb(150, Color.Orange), 1.75f);
    /// <summary>
    /// 节点边框绘制用笔（虚线）
    /// </summary>
    public Pen NodePenDash { get; } = new(Color.FromArgb(150, Color.Orange), 2f)
    {
        DashStyle = System.Drawing.Drawing2D.DashStyle.Custom,
        DashPattern = [1.25f, 1.25f],
    };
    /// <summary>
    /// 坐标辅助线绘制用笔
    /// </summary>
    public Pen GuidePen { get; } = new(Color.FromArgb(200, Color.Red), 1.75f);

    public string LocalName { get; set; } = nameof(GridData);

    public void Serialize(SsSerializer serializer)
    {
    }

    public void Deserialize(SsDeserializer deserializer)
    {
    }
}