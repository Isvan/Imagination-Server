﻿using System.Collections.Generic;

namespace ImaginationServer.Common.Data
{
    public class Character
    {
        public virtual long Id { get; set; }

        public virtual string Owner { get; set; }
       
        public virtual int GmLevel { get; set; }
        public virtual long Reputation { get; set; }

        public virtual string Name { get; set; }
        public virtual string UnapprovedName { get; set; }
        public virtual bool NameRejected { get; set; }
        public virtual bool FreeToPlay { get; set; }

        public virtual uint ShirtColor { get; set; }
        public virtual uint ShirtStyle { get; set; }
        public virtual uint PantsColor { get; set; }
        public virtual uint HairStyle { get; set; }
        public virtual uint HairColor { get; set; }
        public virtual uint Lh { get; set; }
        public virtual uint Rh { get; set; }
        public virtual uint Eyebrows { get; set; }
        public virtual uint Eyes { get; set; }
        public virtual uint Mouth { get; set; }

        public virtual IList<BackpackItem> Items { get; set; }
        public virtual uint Level { get; set; }
        public virtual IList<string> Missions { get; set; }

        public virtual ushort LastZoneId { get; set; }
        public virtual ushort MapInstance { get; set; }
        public virtual uint MapClone { get; set; }
        public virtual float[] Position { get; set; }

        public static long GetObjectId(Character character)
        {
            return character.Id + 1152921504606846994;
        }
    }
}