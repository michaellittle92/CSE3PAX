namespace CSE3PAX
{
    public static class SessionExtensions
    {
        public static void SetBoolean(this ISession session, string key, bool value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        public static bool GetBoolean(this ISession session, string key)
        {
            var data = session.Get(key);
            return data != null && BitConverter.ToBoolean(data, 0);
        }

        public static void SetDateTime(this ISession session, string key, DateTime value)
        {
            var ticks = BitConverter.GetBytes(value.Ticks);
            session.Set(key, ticks);
        }

        public static DateTime GetDateTime(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return default; // Or throw an exception
            }
            var ticks = BitConverter.ToInt64(data, 0);
            return new DateTime(ticks);
        }
    }
}
