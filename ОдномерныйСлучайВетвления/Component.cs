namespace �������������������������
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// ������������ ����� ���� �� ������ ���������� ����������.
    /// </summary>
    public class Component : IComparable<Component>
    {
        /// <summary>
        /// ����������� ���������� ������������� ����� ����������.
        /// </summary>
        /// <param name="old">������������ ���� ����������</param>
        public Component(Component old)
        {
            this.Deg = old.Deg;
            this.K = old.K;
        }

        /// <summary>
        /// ������� ����� ���� ����������.
        /// </summary>
        /// <param name="deg">������� �����.</param>
        /// <param name="k">����������� ����� ������</param>
        public Component(double deg, double k)
        {
            this.K = k;
            this.Deg = deg;
        }

        /// <summary>
        /// ������� ����� ����� � ����������.
        /// </summary>
        public double Deg { get; private set; }

        /// <summary>
        /// ����������� ����� ����� � ����������.
        /// </summary>
        public double K { get; set; }

        int IComparable<Component>.CompareTo(Component other)
        {
            return this.Deg.CompareTo(other.Deg);
        }

        /// <summary>
        /// ���������� ����� ���� ����������, ������ �����, ������������ � �������.
        /// </summary>
        /// <param name="e">������� ���������� ����� ����������.</param>
        /// <returns></returns>
        public Component Pow(double e)
        {
            return new Component(this.Deg * e, Math.Pow(this.K, e));
        }

        /// <summary>
        /// ���������� ����� ���� ����������, ������ ��������� ����� ����� ���������� �� ������.
        /// </summary>
        /// <param name="other">������ ���� ����������, �� ������� ���������� ��������.</param>
        /// <returns></returns>
        public Component Multiply(Component other)
        {
            return new Component(this.Deg + other.Deg, this.K * other.K);
        }

        public double Value(double x)
        {
            return this.K * Math.Pow(x, this.Deg);
        }
    }
}