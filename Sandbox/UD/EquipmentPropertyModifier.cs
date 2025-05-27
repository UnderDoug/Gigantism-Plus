using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;

namespace XRL.World.Parts
{
    [Serializable]
    public class EquipmentPropertyModifier : IPart
    {
        public string Props;

        public override bool SameAs(IPart p)
        {
            return false;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == EquippedEvent.ID
                || ID == UnequippedEvent.ID
                || ID == PooledEvent<GetJumpingBehaviorEvent>.ID
                || ID == PooledEvent<GetPropertyModDescription>.ID;
        }

        public override bool HandleEvent(GetPropertyModDescription E)
        {
            if (E.Actor == ParentObject.Equipped && ParseProps(Props).TryGetValue(E.Property, out var value))
            {
                E.AddLinearBonusModifier(value, ParentObject.BaseDisplayName);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetJumpingBehaviorEvent E)
        {
            if (E.Actor == ParentObject.Equipped && E.Stats != null && ParseProps(Props).TryGetValue("JumpRangeModifier", out var value))
            {
                E.Stats.AddLinearBonusModifier("Range", value, ParentObject.BaseDisplayName);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(EquippedEvent E)
        {
            foreach (KeyValuePair<string, int> item in ParseProps(Props))
            {
                E.Actor.ModIntProperty(item.Key, item.Value, RemoveIfZero: true);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(UnequippedEvent E)
        {
            foreach (KeyValuePair<string, int> item in ParseProps(Props))
            {
                E.Actor.ModIntProperty(item.Key, -item.Value, RemoveIfZero: true);
            }
            return base.HandleEvent(E);
        }

        public Dictionary<string, int> ParseProps(string Props)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            string[] array = Props.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(':');
                dictionary.Add(array2[0], Convert.ToInt32(array2[1]));
            }
            return dictionary;
        }
    }

}
