namespace Logic
{
    public static class Api
    {
        public static IController CreateController()
        {
            return new Controller();
        }
    }
}
