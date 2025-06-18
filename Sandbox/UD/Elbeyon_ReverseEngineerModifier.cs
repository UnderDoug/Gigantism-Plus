using System;
using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.Wish;
using XRL.UI;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class Elbeyon_ReverseEngineerModifier : IPlayerPart
    {
        public int ChanceIn100;

        public bool BlockOtherBonuses;

        public bool OnlyBlockIfChanceIs0;

        public Elbeyon_ReverseEngineerModifier()
        {
            ChanceIn100 = 15; // Change this to whatever you like.
            BlockOtherBonuses = true;
            OnlyBlockIfChanceIs0 = true;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            // Registrar.Register(GetTinkeringBonusEvent.ID, EventOrder.EXTREMELY_EARLY);
            base.Register(Object, Registrar);
        }
        public override bool HandleEvent(GetTinkeringBonusEvent E)
        {
            if (E.Type == "ReverseEngineer")
            {
                E.Bonus = ChanceIn100 - E.BaseRating;

                if (BlockOtherBonuses && (OnlyBlockIfChanceIs0 == (ChanceIn100 == 0)))
                {
                    return true;
                }
            }
            return base.HandleEvent(E);
        }

        // [WishCommand(Command = "remove Elbeyon_ReverseEngineerModifier")]
        public static void Remove_Elbeyon_ReverseEngineerModifier_WishHandler()
        {
            The.Player.RemovePart<Elbeyon_ReverseEngineerModifier>();
            if (!The.Player.HasPart<Elbeyon_ReverseEngineerModifier>())
            {
                Popup.Show($"{nameof(Elbeyon_ReverseEngineerModifier)} removed, you should save the game and disable the mod otherwise it'll be back!");
            }
            else
            {
                Popup.Show($"Something went wrong removing {nameof(ChanceIn100)}, check Player.log for errors.");
            }
        }

        // [WishCommand(Command = "set Elbeyon_ReverseEngineerModifier")]
        public static void Set_Elbeyon_ReverseEngineerModifier_WishHandler(string ChanceIn100)
        {
            if (!ChanceIn100.IsNullOrEmpty() && int.TryParse(ChanceIn100, out int chanceIn100))
            {
                if (!The.Player.TryGetPart(out Elbeyon_ReverseEngineerModifier rEModifier))
                {
                    rEModifier = The.Player.RequirePart<Elbeyon_ReverseEngineerModifier>();
                }
                rEModifier.ChanceIn100 = Math.Max(0, Math.Min(chanceIn100, 100)); // Not less than 0, not more than 100.
                Popup.Show($"{nameof(rEModifier.ChanceIn100)} set to {rEModifier.ChanceIn100}.");
            }
            else
            {
                Popup.Show($"Couldn't convert \"{ChanceIn100}\" into a number!");
            }
        }
    }
}

namespace Elbeyon_Mods
{
    [PlayerMutator]
    public class ElbeyonPlayerMutator : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            // Gets called once when the player is first generated

            // player.RequirePart<Elbeyon_ReverseEngineerModifier>();
        }
    }

    [HasCallAfterGameLoaded]
    public class ElbeyonOnLoadGameHandler
    {
        [CallAfterGameLoaded]
        public static void OnLoadGameCallback()
        {
            // Gets called every time the game is loaded (from a save) but not during generation

            // The.Player.RequirePart<Elbeyon_ReverseEngineerModifier>();
        }
    }
}
