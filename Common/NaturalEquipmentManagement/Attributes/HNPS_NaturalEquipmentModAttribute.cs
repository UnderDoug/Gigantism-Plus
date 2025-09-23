using System;
using System.Collections.Generic;
using System.Text;

namespace HNPS_GigantismPlus
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HNPS_NaturalEquipmentModAttribute : Attribute
    {
        public string ForBodyPart;
        public string[] ForBodyParts;
        public bool PassLevel;

        public HNPS_NaturalEquipmentModAttribute()
        {
            ForBodyPart = null;
            ForBodyParts = null;
            PassLevel = false;
        }
        public HNPS_NaturalEquipmentModAttribute(bool PassLevel)
            : this()
        {
            this.PassLevel = PassLevel;
        }
        public HNPS_NaturalEquipmentModAttribute(string ForBodyPart, bool PassLevel = false)
            : this(PassLevel)
        {
            this.ForBodyPart = ForBodyPart;
        }
        public HNPS_NaturalEquipmentModAttribute(string[] ForBodyParts, bool PassLevel = false)
            : this(PassLevel)
        {
            this.ForBodyParts = ForBodyParts;
        }
    }
}
