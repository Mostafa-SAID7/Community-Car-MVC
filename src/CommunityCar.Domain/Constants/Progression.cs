namespace CommunityCar.Domain.Constants;

public static class Progression
{
    public static class Thresholds
    {
        public const int User = 0;
        public const int Expert = 5000;
        public const int Reviewer = 10000;
        public const int Author = 15000;
        public const int Master = 20000;
    }

    public static class DailyLimits
    {
        public static class Expert
        {
            public const int Guides = 5;
        }

        public static class Reviewer
        {
            public const int Reviews = 5;
            public const int Guides = 10;
        }

        public static class Author
        {
            public const int Articles = 5;
            public const int Reviews = 10;
            public const int Guides = 20;
        }
    }
}
