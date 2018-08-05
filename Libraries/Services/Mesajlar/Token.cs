namespace Services.Mesajlar
{
    public sealed class Token
    {
        #region Fields

        private readonly string _key;
        private readonly object _value;
        private readonly bool _neverHtmlEncoded;

        #endregion

        #region Ctors

        public Token(string key, object value, bool neverHtmlEncoded)
        {
            this._key = key;
            this._value = value;
            this._neverHtmlEncoded = neverHtmlEncoded;
        }

        public Token(string key, object value) : this(key, value, false)
        {
        }

        #endregion

        #region Properties
        public string Key { get { return _key; } }
        public object Value { get { return _value; } }
        public bool NeverHtmlEncoded { get { return _neverHtmlEncoded; } }

        #endregion

        #region Methods
        public override string ToString()
        {
            return string.Format("{0}: {1}", Key, Value);
        }

        #endregion
    }

}
