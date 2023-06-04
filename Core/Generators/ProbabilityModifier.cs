namespace SuspiciousGames.SellMe.Core.Generators
{
    public enum ModifierType : byte
    {
        DistributeToOthers,
        Mult,
        Add,
        Set
    }

    public class ProbabilityModifier<T>
    {
        public ModifierType modifierType;
        public T dataToModify;
        public float value;

        public ProbabilityModifier(ModifierType modifierType, T dataToModify, float value)
        {
            this.modifierType = modifierType;
            this.dataToModify = dataToModify;
            this.value = value;
        }

        public ProbabilityModifier(ProbabilityModifier<T> probMod)
        {
            modifierType = probMod.modifierType;
            dataToModify = probMod.dataToModify;
            value = probMod.value;
        }
    }
}
