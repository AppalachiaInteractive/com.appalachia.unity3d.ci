using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public struct PrivateUnion
    {
        public bool? Bool;
        public PrivateEnum? Enum;

        public static implicit operator PrivateUnion(bool Bool)
        {
            return new() {Bool = Bool};
        }

        public static implicit operator PrivateUnion(PrivateEnum Enum)
        {
            return new() {Enum = Enum};
        }
    }
}
