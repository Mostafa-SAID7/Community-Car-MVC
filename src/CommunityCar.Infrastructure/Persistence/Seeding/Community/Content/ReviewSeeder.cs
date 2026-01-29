using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Community.Content;

public class ReviewSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ReviewSeeder> _logger;

    public ReviewSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        ILogger<ReviewSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Reviews.AnyAsync()) return;

        _logger.LogInformation("Seeding reviews...");

        var users = await _userManager.Users.ToListAsync();
        var reviews = new List<CommunityCar.Domain.Entities.Community.Reviews.Review>();
        var random = new Random();

        // Sample review data
        var reviewData = new[]
        {
            new {
                Title = "Excellent Electric Vehicle Experience",
                Comment = @"I've owned this Tesla Model 3 for over a year now, and it has exceeded all my expectations. The acceleration is incredible, the autopilot features work flawlessly on highways, and the over-the-air updates keep improving the car. 

The build quality is solid, and I haven't had any major issues. The charging network is extensive, making long trips worry-free. The interior is minimalist but functional, though some might miss physical buttons.

Overall, this is the best car I've ever owned. The combination of performance, technology, and environmental benefits makes it a clear winner.",
                Rating = 5,
                CarMake = "Tesla",
                CarModel = "Model 3",
                CarYear = 2023,
                IsVerifiedPurchase = true,
                IsRecommended = true,
                Mileage = 15000,
                OwnershipDuration = "1 year",
                QualityRating = 5,
                ValueRating = 4,
                ReliabilityRating = 5,
                PerformanceRating = 5,
                ComfortRating = 4,
                Pros = new[] { "Incredible acceleration", "Advanced autopilot", "Over-the-air updates", "Excellent charging network", "Zero emissions" },
                Cons = new[] { "Minimalist interior might not appeal to everyone", "Road noise at highway speeds" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1560958089-b8a1929cea89?w=800&h=400&fit=crop" }
            },
            new {
                Title = "Reliable Daily Driver with Great Fuel Economy",
                Comment = @"My Honda Civic has been a fantastic daily driver for the past three years. It's incredibly reliable, fuel-efficient, and comfortable for both city and highway driving.

The interior is well-designed with intuitive controls and good build quality. The infotainment system works well, though it could be more responsive. The trunk space is adequate for a compact car.

Maintenance costs have been very reasonable, and I've had no major repairs. The CVT transmission is smooth, though some might find it less engaging than a traditional automatic.

For the price point, this car offers excellent value and I would definitely recommend it to anyone looking for a practical, efficient vehicle.",
                Rating = 4,
                CarMake = "Honda",
                CarModel = "Civic",
                CarYear = 2021,
                IsVerifiedPurchase = true,
                IsRecommended = true,
                Mileage = 45000,
                OwnershipDuration = "3 years",
                QualityRating = 4,
                ValueRating = 5,
                ReliabilityRating = 5,
                PerformanceRating = 3,
                ComfortRating = 4,
                Pros = new[] { "Excellent fuel economy", "Very reliable", "Comfortable interior", "Low maintenance costs", "Good resale value" },
                Cons = new[] { "CVT can feel sluggish", "Road noise on rough surfaces", "Rear seat space could be better" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800&h=400&fit=crop" }
            },
            new {
                Title = "Powerful Truck with Some Drawbacks",
                Comment = @"The F-150 is undoubtedly a capable truck with impressive towing capacity and a comfortable cabin. The EcoBoost engine provides good power and reasonable fuel economy for a full-size truck.

The interior is spacious and well-appointed, with plenty of storage options. The infotainment system is user-friendly, and the driver assistance features work well.

However, I've experienced some reliability issues with the turbocharger, and the repair costs were significant. The ride quality can be harsh when the truck is unloaded, and fuel economy drops significantly when towing.

Despite these issues, it's still a solid choice for those who need a capable work truck, but I'd recommend considering the warranty carefully.",
                Rating = 3,
                CarMake = "Ford",
                CarModel = "F-150",
                CarYear = 2020,
                IsVerifiedPurchase = true,
                IsRecommended = false,
                Mileage = 65000,
                OwnershipDuration = "2.5 years",
                QualityRating = 3,
                ValueRating = 3,
                ReliabilityRating = 2,
                PerformanceRating = 4,
                ComfortRating = 4,
                Pros = new[] { "Excellent towing capacity", "Spacious cabin", "Good infotainment system", "Strong engine options" },
                Cons = new[] { "Reliability concerns with turbo", "High repair costs", "Poor fuel economy when towing", "Harsh ride when unloaded" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&h=400&fit=crop" }
            },
            new {
                Title = "Luxury and Performance Combined",
                Comment = @"The BMW 3 Series delivers an exceptional driving experience with its perfect balance of luxury and performance. The handling is precise, the engine is smooth and powerful, and the interior quality is top-notch.

The iDrive system has improved significantly and is now intuitive to use. The seats are comfortable for long drives, and the build quality feels solid throughout.

However, maintenance costs are high, and some features that should be standard are expensive options. The run-flat tires are noisy and expensive to replace.

If you can afford the premium maintenance costs, this is an excellent luxury sport sedan that delivers on BMW's promise of the ultimate driving machine.",
                Rating = 4,
                CarMake = "BMW",
                CarModel = "3 Series",
                CarYear = 2022,
                IsVerifiedPurchase = true,
                IsRecommended = true,
                Mileage = 25000,
                OwnershipDuration = "1.5 years",
                QualityRating = 5,
                ValueRating = 3,
                ReliabilityRating = 4,
                PerformanceRating = 5,
                ComfortRating = 5,
                Pros = new[] { "Excellent handling", "Powerful engine", "Luxury interior", "Advanced technology", "Strong performance" },
                Cons = new[] { "High maintenance costs", "Expensive options", "Noisy run-flat tires", "Premium fuel required" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1555215695-3004980ad54e?w=800&h=400&fit=crop" }
            },
            new {
                Title = "Great Value SUV for Families",
                Comment = @"The Hyundai Tucson has been an excellent family vehicle. It offers a good balance of features, comfort, and value. The interior is spacious enough for our family of four, and the cargo area is adequate for our needs.

The infotainment system is easy to use, and the warranty coverage provides peace of mind. Fuel economy is decent for an SUV, and the ride quality is comfortable on most road surfaces.

The styling is modern and attractive, and we've received many compliments on the design. Build quality seems solid so far, though it's still relatively new to us.

For the price, this SUV offers a lot of value and would be a good choice for families looking for a reliable, well-equipped vehicle.",
                Rating = 4,
                CarMake = "Hyundai",
                CarModel = "Tucson",
                CarYear = 2023,
                IsVerifiedPurchase = true,
                IsRecommended = true,
                Mileage = 8000,
                OwnershipDuration = "8 months",
                QualityRating = 4,
                ValueRating = 5,
                ReliabilityRating = 4,
                PerformanceRating = 3,
                ComfortRating = 4,
                Pros = new[] { "Great value for money", "Spacious interior", "Good warranty", "Modern styling", "User-friendly tech" },
                Cons = new[] { "Engine could be more powerful", "Some interior materials feel cheap", "Road noise at highway speeds" },
                ImageUrls = new[] { "https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800&h=400&fit=crop" }
            }
        };

        // Create reviews from sample data
        foreach (var data in reviewData)
        {
            var randomReviewer = users[random.Next(users.Count)];
            var targetId = Guid.NewGuid(); // In a real scenario, this would be a vehicle ID
            
            var review = new CommunityCar.Domain.Entities.Community.Reviews.Review(
                targetId, "Vehicle", data.Rating, data.Title, data.Comment, randomReviewer.Id);

            // Set purchase info
            review.SetPurchaseInfo(data.IsVerifiedPurchase, DateTime.Now.AddMonths(-random.Next(6, 36)), random.Next(20000, 80000));
            review.SetRecommendation(data.IsRecommended);
            review.SetCarInfo(data.CarMake, data.CarModel, data.CarYear, data.Mileage, data.OwnershipDuration);
            review.SetDetailedRatings(data.QualityRating, data.ValueRating, data.ReliabilityRating, data.PerformanceRating, data.ComfortRating);

            // Add images
            foreach (var imageUrl in data.ImageUrls)
            {
                review.AddImage(imageUrl);
            }

            // Add pros
            foreach (var pro in data.Pros)
            {
                review.AddPro(pro);
            }

            // Add cons
            foreach (var con in data.Cons)
            {
                review.AddCon(con);
            }

            // Approve the review
            review.Approve();

            // Add some engagement
            var viewCount = random.Next(50, 500);
            for (int i = 0; i < viewCount; i++)
            {
                review.IncrementViewCount();
            }

            var helpfulCount = random.Next(5, 50);
            for (int i = 0; i < helpfulCount; i++)
            {
                review.IncrementHelpfulCount();
            }

            reviews.Add(review);
        }

        // Create additional random reviews
        var carMakes = new[] { "Toyota", "Honda", "Ford", "BMW", "Mercedes", "Audi", "Volkswagen", "Nissan", "Hyundai", "Kia", "Mazda", "Subaru", "Chevrolet", "Jeep", "Volvo" };
        var carModels = new[] { "Sedan", "SUV", "Hatchback", "Coupe", "Wagon", "Crossover", "Pickup", "Convertible" };
        var reviewTitles = new[]
        {
            "Great car for the money", "Disappointed with reliability", "Excellent fuel economy", "Comfortable and spacious",
            "Poor build quality", "Outstanding performance", "Good value proposition", "Maintenance nightmare",
            "Perfect family car", "Not worth the premium", "Reliable daily driver", "Impressive technology features",
            "Overpriced for what you get", "Exceeded expectations", "Solid choice for commuting"
        };

        for (int i = 0; i < 25; i++)
        {
            var randomReviewer = users[random.Next(users.Count)];
            var targetId = Guid.NewGuid();
            var rating = random.Next(1, 6);
            var title = reviewTitles[random.Next(reviewTitles.Length)];
            var carMake = carMakes[random.Next(carMakes.Length)];
            var carModel = carModels[random.Next(carModels.Length)];
            var carYear = random.Next(2018, 2025);

            var comment = GenerateRandomReviewComment(rating, carMake, carModel);

            var review = new CommunityCar.Domain.Entities.Community.Reviews.Review(
                targetId, "Vehicle", rating, title, comment, randomReviewer.Id);

            // Set car info
            review.SetCarInfo(carMake, carModel, carYear, random.Next(5000, 100000), $"{random.Next(6, 60)} months");

            // 70% chance of verified purchase
            if (random.Next(100) < 70)
            {
                review.SetPurchaseInfo(true, DateTime.Now.AddMonths(-random.Next(1, 36)), random.Next(15000, 75000));
            }

            // 60% chance of recommendation for ratings 4-5, 20% for rating 3, 5% for ratings 1-2
            var recommendChance = rating >= 4 ? 60 : rating == 3 ? 20 : 5;
            if (random.Next(100) < recommendChance)
            {
                review.SetRecommendation(true);
            }

            // Add detailed ratings (50% chance)
            if (random.Next(100) < 50)
            {
                review.SetDetailedRatings(
                    random.Next(rating - 1, rating + 2),
                    random.Next(rating - 1, rating + 2),
                    random.Next(rating - 1, rating + 2),
                    random.Next(rating - 1, rating + 2),
                    random.Next(rating - 1, rating + 2)
                );
            }

            // Add some pros and cons (30% chance)
            if (random.Next(100) < 30)
            {
                var commonPros = new[] { "Good fuel economy", "Comfortable seats", "Reliable", "Good value", "Easy to drive", "Spacious interior" };
                var commonCons = new[] { "Road noise", "Poor infotainment", "Expensive maintenance", "Limited cargo space", "Harsh ride", "Cheap interior materials" };

                var prosCount = random.Next(1, 4);
                for (int j = 0; j < prosCount; j++)
                {
                    review.AddPro(commonPros[random.Next(commonPros.Length)]);
                }

                var consCount = random.Next(1, 3);
                for (int j = 0; j < consCount; j++)
                {
                    review.AddCon(commonCons[random.Next(commonCons.Length)]);
                }
            }

            // Approve most reviews (90%)
            if (random.Next(100) < 90)
            {
                review.Approve();
            }

            // Add engagement
            var views = random.Next(10, 200);
            for (int j = 0; j < views; j++)
            {
                review.IncrementViewCount();
            }

            var helpful = random.Next(0, 20);
            for (int j = 0; j < helpful; j++)
            {
                review.IncrementHelpfulCount();
            }

            reviews.Add(review);
        }

        await _context.Reviews.AddRangeAsync(reviews);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Successfully seeded {reviews.Count} reviews.");
    }

    private static string GenerateRandomReviewComment(int rating, string carMake, string carModel)
    {
        var positiveComments = new[]
        {
            $"I've been very happy with my {carMake} {carModel}. It's reliable, efficient, and comfortable for daily driving.",
            $"This {carMake} {carModel} has exceeded my expectations. Great build quality and excellent features for the price.",
            $"Highly recommend this {carMake} {carModel}. It's been trouble-free and a pleasure to drive.",
            $"The {carMake} {carModel} is an excellent choice. Good performance, comfort, and value for money."
        };

        var neutralComments = new[]
        {
            $"The {carMake} {carModel} is decent overall. Some good points and some areas for improvement.",
            $"Mixed feelings about this {carMake} {carModel}. It does the job but has some quirks.",
            $"The {carMake} {carModel} is okay for the price. Not outstanding but gets the job done.",
            $"Average experience with this {carMake} {carModel}. Some things I like, others not so much."
        };

        var negativeComments = new[]
        {
            $"Disappointed with this {carMake} {carModel}. Several issues and higher maintenance costs than expected.",
            $"Would not recommend this {carMake} {carModel}. Poor reliability and expensive repairs.",
            $"The {carMake} {carModel} has been problematic. Multiple trips to the dealer for various issues.",
            $"Not impressed with this {carMake} {carModel}. Build quality is poor and it feels cheap."
        };

        return rating >= 4 ? positiveComments[new Random().Next(positiveComments.Length)] :
               rating == 3 ? neutralComments[new Random().Next(neutralComments.Length)] :
               negativeComments[new Random().Next(negativeComments.Length)];
    }
}