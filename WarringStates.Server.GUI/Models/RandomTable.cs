﻿using LocalUtilities.SimpleScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Server.GUI.Models;

internal class RandomTable
{
    public double[] Table { get; private set; } = [];

    [SsIgnore]
    int Index { get; set; } = 0;

    public RandomTable(int number)
    {
        Table = new double[number];
        var random = new Random();
        for (int i = 0; i < number; i++)
            Table[i] = random.NextDouble();
    }

    public RandomTable(double[] table)
    {
        Table = table;
    }

    public RandomTable()
    {

    }

    public double Next()
    {
        Index++;
        Index = Index < Table.Length ? Index : 0;
        return Table[Index];
    }

    public void ResetIndex()
    {
        Index = 0;
    }

    public double Current()
    {
        return Table[Index];
    }
}