
namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
    public class QualifiedRandom
    {
        // The number of random elements in X.
        public int a { get; internal set; }

        // The random variable X, a * x, x | [0..255]
        public byte[] X { get; internal set; }

        // The sum 0..a of x.
        public int Xa { get; internal set; }

        // The expected value of X, E_Xa = 256 / 2 * a,  X | discrete uniformly distibuted
        public int E_Xa { get; internal set; }

        // The sample size.
        public int k { get; internal set; }
    }
}
