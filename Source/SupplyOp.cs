﻿using System;
using Chainly;

namespace Revital
{
    public struct SupplyOp : IData
    {
        short state;
        string label;
        int orgid;
        int uid;
        string uname;
        string utel;
        DateTime stamp;

        public void Read(ISource s, short msk = 255)
        {
        }

        public void Write(ISink s, short msk = 255)
        {
        }
    }
}