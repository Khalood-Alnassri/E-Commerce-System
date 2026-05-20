using Azure;
using E_Commerce_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace E_Commerce_System
{
    internal class Program
    {
        static ApplicationDbContext context = new ApplicationDbContext();
        static int currentUserId = 0;
        static string currentUserRole = "";
        private static object user;

        // function helper to hash password
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        // helper function to check user name
        public static bool CheckUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                Console.WriteLine("Name are required. ");
                return false;
            }

            return true;
        }

        // helper function to check user email
        public static bool CheckUserEmail(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                Console.WriteLine("Email are required. ");
                return false;
            }

            return true;
        }

        // helper function to check regex email
        public static bool CheckRegex(string Email)
        {
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                Console.WriteLine("Invalid email format.");
                return false;
            }
            return true;
        }

        // helper function to check user role
        public static string CheckUserRole(string userRole)
        {
            if (userRole.ToLower() == "user")
            {
                return "User";
            }

            else if (userRole.ToLower() == "admin")
            {
                return "Admin";
            }

            return "Invide role";
        }

        // case 1: regist user 
        static void Register()
        {
            Console.WriteLine("====REGISTER====");

            Console.Write("Enter Name: ");
            string uName = Console.ReadLine()?.Trim();

            CheckUserName(uName);

            Console.Write("Enter Email: ");
            string uEmail = Console.ReadLine()?.Trim();

            CheckUserEmail(uEmail);

            CheckRegex(uEmail);
          
            var existingUser = context.Users.FirstOrDefault(u => u.Email == uEmail);
            if (existingUser != null)
            {
                Console.WriteLine("Email already exists. ");
                return;
            }

            Console.Write("Enter Password: ");
            string uPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(uPassword) || uPassword.Length < 6)
            {
                Console.WriteLine("Password must be at least 6 characters.");
                return;
            }

            Console.Write("Enter Phone Number: ");
            string uPhone = Console.ReadLine()?.Trim();

            Console.WriteLine("Choose role: ");
            string role = Console.ReadLine()?.Trim();

            // check user role
            CheckUserRole(role);

            var user = new User
            {
                UName = uName,
                Email = uEmail,
                Password = HashPassword (uPassword),
                Phone = uPhone,
                Role = role,
                CreatedAt = DateTime.Now
            };

            context.Users.Add(user);
            context.SaveChanges();

            Console.WriteLine("Registration successful");
        }

        // case 2: user login to the system
        static void Login()
        {
            Console.WriteLine("===== Login =====");

            Console.Write("Enter your Email: ");
            string logEmail = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter your password: ");
            string logPassword = Console.ReadLine();

            // check user email
            CheckUserEmail(logEmail);
            CheckRegex(logEmail);

            var user = context.Users.FirstOrDefault(u => u.Email == logEmail);

            if (string.IsNullOrWhiteSpace(logPassword))
            {
                Console.WriteLine("Email and Password are required. ");
                return;
            }

            // hash log password
            string hashedPassword = HashPassword(logPassword);

            if (user.Password != hashedPassword)
            {
                Console.WriteLine("Invalid password");
                return;
            }

            currentUserId = user.UId;
            currentUserRole = user.Role;
            Console.WriteLine($"Welcome, {user.UName}");
        }
       
        // case 3: Get user details
        public static void UserInformation()
        {
            var currentUser = context.Users.FirstOrDefault(u => u.UId == currentUserId);
            Console.WriteLine("User Info:");
            Console.WriteLine("Name: " + currentUser.UName);
            Console.WriteLine("Email: " + currentUser.Email);
            Console.WriteLine("Phone: " + currentUser.Phone);
            Console.WriteLine("Role: " + currentUser.Role);
        }

        // case 4: Add a new product
        public static void AddProduct() 
        {
            // check admin role
            if (currentUserRole != "Admin")
            {
                Console.WriteLine("Access denied. Only admins can add products.");
                return;
            }

            Console.WriteLine("Enter product name: ");
            string prodName = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter product description: ");
            string descript = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter product price");
            decimal price = decimal.Parse(Console.ReadLine()?.Trim());

            if(price <= 0) // check price
            {
                Console.WriteLine("Product price should be greater than zero.");
                return;
            }

            Console.WriteLine("Enter product stock: ");
            int stock = int.Parse(Console.ReadLine()?.Trim());

            if (stock < 0) // check stock
            {
                Console.WriteLine("Product price should be greater than or equal zero.");
                return;
            } 

            // add product
            context.Products.Add( new Product { PName = prodName, Description = descript, Price = price, Stock = stock });
            context.SaveChanges(); // save change in database
            Console.WriteLine("Product added successfully.");
        }

        // case 5: Update product details
        public static void UpdateProduct()
        {
            // check admin role
            if (currentUserRole != "Admin")
            {
                Console.WriteLine("Access denied. Only admins can add products.");
                return;
            }

            Console.WriteLine("Enter product ID: ");
            int prodId = int.Parse(Console.ReadLine()?.Trim());

            Product product = context.Products.Find(prodId); 

            if(product != null)
            {
                Console.WriteLine("Enter new product price");
                decimal price = decimal.Parse(Console.ReadLine()?.Trim());

                if (price <= 0) // check the price
                {
                    Console.WriteLine("Product price should be greater than zero.");
                    return;
                }
                
                // replace price
                product.Price = price;

                Console.WriteLine("Enter new product stock: ");
                int stock = int.Parse(Console.ReadLine()?.Trim());

                if (stock < 0) // check stock
                {
                    Console.WriteLine("Product price should be greater than or equal zero.");
                    return;
                }

                // replace stock
                product.Stock = stock;

                context.Products.Update(product); // update the product
                context.SaveChanges(); // save change in database
                Console.WriteLine("Product update successfully.");
            }

            else
            {
                Console.WriteLine("Product with " + prodId + " not found.");
                return;
            }
        }

        // case 6:  Get a list of products
        public static void ListOfProducts()
        {
            // count all products
            var productCount = context.Products.Count();

            // user in page number 
            Console.WriteLine("Enter page number: ");
            int page = int.Parse(Console.ReadLine()); 

            int pageSize = 10; // each page have 10 products
            var products = context.Products.Select(p => new { p.PName, p.Stock, p.Price })
                                           .OrderBy(p => p.PName)
                                           .Skip((page - 1) * pageSize)
                                           .Take(pageSize).ToList();

            foreach (var product in products)
            {
                Console.WriteLine("=======================");
                Console.WriteLine("Name: " +product.PName);
                Console.WriteLine("Price: " + product.Price);
                Console.WriteLine("Stock: " + product.Stock);
            }
        }

        // case 7: Get product details by ID
        public static void ProductDetail()
        {
            // loop and list all product with id and name
            var prod = context.Products.ToList();
            foreach(var p in prod) 
            {
                Console.WriteLine("- Product ID: " + p.PId + " | Product name: " + p.PName);
            }

            // take product id from the user 
            Console.WriteLine("Enter product ID: ");
            int productId = int.Parse(Console.ReadLine()?.Trim());

            // search for the product
            var product = context.Products.Select(p => new { p.PId, p.PName, p.Price, p.Description, p.Stock })
                                          .FirstOrDefault(p => p.PId == productId);

            if (product != null)
            {
                Console.WriteLine("Product Information: ");
                Console.WriteLine("Name: " + product.PName);
                Console.WriteLine("Description: " + product.Description);
                Console.WriteLine("Stock: " + product.Stock);
                Console.WriteLine("Price: " + product.Price);
            }

            else
            {
                Console.WriteLine("Product with " + productId + " not found.");
                return;
            }
        }

        // case 8:  Place a new order
        public static void PlaceNewOrder()
        {
            // check user login
            if (currentUserId == 0)
            {
                Console.WriteLine("You must login first.");
                return;
            }

            // add order in Oredr table
            var order = new Order
            {
                UId = currentUserId,
                OrderDate = DateTime.Now
            };

            context.Orders.Add(order);
            context.SaveChanges();

            bool adding = true;

            while (adding)
            {
                Console.WriteLine("=== Available Products ===");

                // list all product with details
                var products = context.Products.ToList();

                foreach (var p in products)
                {
                    Console.WriteLine($"ID: {p.PId} | Name: {p.PName} | Price: {p.Price} | Stock: {p.Stock}");
                }

                // take product ID from the user
                Console.Write("Enter Product ID (0 to finish): ");
                int productId = int.Parse(Console.ReadLine());

                if (productId == 0) // if user enter 0 the loop finish
                {
                    break;
                }

                // search for product
                var product = context.Products.FirstOrDefault(p => p.PId == productId);

                if (product == null)
                {
                    Console.WriteLine("Invalid product.");
                    continue;
                }

                Console.Write("Enter Quantity: ");
                int qty = int.Parse(Console.ReadLine());

                // check quantity
                if (qty <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                // check product stock
                if (product.Stock < qty)
                {
                    Console.WriteLine($"Cannot place order. Only {product.Stock} items available in stock.");

                    return; // stop entire order
                }

                var orderProduct = new OrderProduct
                {
                    OId = order.OId,
                    PId = product.PId,
                    Quantity = qty
                };

                context.OrderProducts.Add(orderProduct); // add order in OrderProducts table

                // calculate the total amount 
                order.TotalAmount = order.OrderProducts
                         .Sum(op => op.Quantity * op.Product.Price);

                // reduce product stock
                product.Stock -= qty;

                Console.WriteLine("Product added.");
            }

            context.SaveChanges(); // save change in database

            Console.WriteLine("===== ORDER COMPLETED =====");
            Console.WriteLine($"Order ID: {order.OId}");
            Console.WriteLine($"Total Amount: {order.TotalAmount}");
        }

        // case 9: Get all orders for a user
        public static void GetUserOrders()
        {
            // search for all orders where user id equal current user id
            var orders = context.Orders.Where(o => o.UId == currentUserId)
                                       .OrderByDescending(o => o.OrderDate)
                                       .ToList();
            Console.WriteLine("=== My Orders ===");
            if (!orders.Any()) // if not order 
            {
                Console.WriteLine("No orders yet.");
                return;
            }

            // list all orders 
            for (int i = 0; i < orders.Count; i++)
            {
                Console.WriteLine($"Order: {orders[i].OId}");
                Console.WriteLine($"Total: {orders[i].TotalAmount:C} ");
                Console.WriteLine($"Date: {orders[i].OrderDate:dd/MM/yyyy}");
                Console.WriteLine($"Products: ");
                foreach (var op in orders[i].OrderProducts)
                {
                    Console.WriteLine($" - {op.Product.PName} x {op.Quantity} | {op.Product.Price:C} each");
                }
            }

        }

        // case 10: Get order details
        public static void OrderDetail()
        {
            Console.WriteLine("Enter order ID: ");
            int orderID = int.Parse(Console.ReadLine());

            // search order and include OrderProducts table then include Product table
            var orders = context.Orders.Include(o => o.OrderProducts)
                                         .ThenInclude(p => p.Product)
                                         .FirstOrDefault(o => o.OId == orderID);

            if (orders == null) // if not found
            {
                Console.WriteLine("Order not found!");
                return;
            }

            // print order delails
            Console.WriteLine("Order Date: " + orders.OrderDate);
            Console.WriteLine("Order total amount: " + orders.TotalAmount);
        }

        // case 11: Add a review for a product
        public static void AddReview()
        {
            Console.WriteLine("Enter product ID: ");
            int productId = int.Parse(Console.ReadLine().Trim());

            // search for Product
            Product product = context.Products.Include(p => p.Reviews)
                                              .FirstOrDefault(p =>  p.PId == productId);
            if (product != null)
            {
                Console.WriteLine("Enter Rating (1-5): ");
                int rate = int.Parse(Console.ReadLine().Trim());

                // check rate 
                if ( rate < 1 || rate > 5)
                {
                    Console.WriteLine("Invalid  rating. Please enter a number between 1 and 5.");
                }

                Console.WriteLine("Enter comment: ");
                string comment = Console.ReadLine().Trim();

                DateTime date = DateTime.Now;

                Review review = new Review
                {
                    Rating = rate,
                    Comment = comment,
                    ReviewDate = DateTime.Now,
                    UId = currentUserId,
                    PId = productId
                };

                // add review
                context.Reviews.Add(review);
                context.SaveChanges(); // save change in database 

                // recalculate overall rating
                product.OverallRating = (decimal)product.Reviews
                                                        .Append(review)
                                                        .Average(r => r.Rating);

                // save updated product rating
                context.SaveChanges();

                Console.WriteLine("Review added successfully.");
                Console.WriteLine("Updated Overall Rating: " + product.OverallRating);
            }

            else
            {
                Console.WriteLine("Product with " + productId + " not found.");
                return;
            }

        }

        // case 12: Get all reviews for a product
        public static void GetAllReview()
        {
            // loop and list all product with id and name
            var prod = context.Products.ToList();
            foreach (var p in prod)
            {
                Console.WriteLine("- Product ID: " + p.PId + " | Product name: " + p.PName);
            }

            // take product id from the user
            Console.WriteLine("Enter product ID : ");
            int id = int.Parse(Console.ReadLine().Trim());

            // search for Product
            Product product = context.Products.Include(p => p.Reviews)
                                           .FirstOrDefault(p => p.PId == id);

            if (product != null)
            {
                // take page number from user
                Console.WriteLine("Enter page number: ");
                int page = int.Parse(Console.ReadLine());
                int pageSize = 5;

                var review = product.Reviews.OrderByDescending(r => r.ReviewDate)
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

                // list all reviews for the select product
                foreach (var Review in review)
                {
                    Console.WriteLine("Reviews for: " + product.PName);
                    Console.WriteLine("Rating: " + Review.Rating);
                    Console.WriteLine("Comment: " + Review.Comment);
                    Console.WriteLine("Date of review: " + Review.ReviewDate);
                }
            }

            else
            {
                Console.WriteLine("Product with " + id + " not found.");
            }
        }

        // case 13:  Edit review
        public static void EditReview()
        {
            // search all reviews for the user 
            var myReviews = context.Reviews.Where(r => r.UId == currentUserId).ToList();

            if (!myReviews.Any()) // if no review
            {
                Console.WriteLine("No reviews found.");
                return;
            }

            Console.WriteLine("=== Your Reviews ====");

            // list all reviews
            foreach (var r in myReviews)
            {
                Console.WriteLine($"ID: {r.RId} | Product: {r.PId} | Rating: {r.Rating}");
                Console.WriteLine($"Comment: {r.Comment}");
                Console.WriteLine("--------------------------------");
            }

            // take review id from the user
            Console.Write("Enter Review ID to edit/delet: ");
            int revId = int.Parse(Console.ReadLine());

            var review = context.Reviews.FirstOrDefault(r => r.RId == revId && r.UId == currentUserId);

            if (review == null) // if not found
            {
                Console.WriteLine("Review not found");
                return;
            }

            Console.WriteLine("1. Edit Review");
            Console.WriteLine("2. Delete Review");
            Console.Write("Choose option: ");

            int choice = int.Parse(Console.ReadLine());

            if (choice == 1) // edit review
            {
                Console.Write("Enter new comment: ");
                string newComm = Console.ReadLine();

                Console.Write("Enter new rating (1-5): ");
                int newRate = int.Parse(Console.ReadLine());

                review.Comment = newComm;
                review.Rating = newRate;
                context.SaveChanges();

                Console.WriteLine("Review update successfully.");
            }
            else if (choice == 2) // delete review
            {
                Console.WriteLine("Are you sure want to delet this review? (y/n)");
                string confirm = Console.ReadLine()?.Trim().ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    context.Reviews.Remove(review);
                    context.SaveChanges();

                    Console.WriteLine("Review deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Delete Cancelled.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice");
            }
        }

        // case 14: Exit
        public static bool Exit()
        {
            Console.WriteLine("Are you sure you want to Exit? (yes/no): ");
            string confirmLogout = Console.ReadLine() ?? string.Empty;

            if (confirmLogout == "yes")
            {
                Console.WriteLine("Exiting system...");
                Console.WriteLine("Thank you for using E-Commerce System!");
                return true;
            }
            else
            {
                Console.WriteLine("Exit cancelled. Returning to user menu...");
                return false;
            }
        }

        // function to Logout user 
        public static bool Logout()
        {
            Console.WriteLine("Are you sure you want to logout? (yes/no): ");
            string confirmLogout = Console.ReadLine() ?? string.Empty;

            if (confirmLogout == "yes")
            {
                Console.WriteLine("Thank you for using E-Commerce System!");
                return true;
            }
            else
            {
                Console.WriteLine("Logout cancelled. Returning...");
                return false;
            }
        }

        // function to choose option
        public static int ChooseOption()
        {
            int option;

            while (true)
            {
                Console.Write("Choose option you need: ");
                string input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out option) && option >= 1 && option <= 14)
                {
                    return option;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 14.");
                }
            }
        }

        // system main
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("====================== E-Commerce System ======================");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Get user details.");
                Console.WriteLine("4. Add a new product.");
                Console.WriteLine("5. Update product details.");
                Console.WriteLine("6. Get a list of products.");
                Console.WriteLine("7. Get product details.");
                Console.WriteLine("8. Place a new order.");
                Console.WriteLine("9. Get all orders for a user.");
                Console.WriteLine("10. Get order details.");
                Console.WriteLine("11. Add a review for a product.");
                Console.WriteLine("12. Get all reviews for a product.");
                Console.WriteLine("13. Edit review");
                Console.WriteLine("14. Exit");

                int choice = ChooseOption();

                switch (choice)
                {
                    case 1:

                        Register();

                        break;

                    case 2:

                        Login();

                        break;

                    case 3:

                        Login();
                        UserInformation();
                        Logout();

                        break;

                    case 4:

                        Login();
                        AddProduct();
                        Logout();

                        break;

                    case 5:

                        Login();
                        UpdateProduct();
                        Logout();

                        break;

                    case 6:

                        ListOfProducts();

                        break;


                    case 7:

                        ProductDetail();

                        break;

                    case 8:

                        Login();
                        PlaceNewOrder();
                        Logout();

                        break;

                    case 9:

                        Login();
                        GetUserOrders();
                        Logout();

                        break;

                    case 10:

                        Login();
                        OrderDetail();
                        Logout();

                        break;

                    case 11:

                        Login();
                        AddReview();
                        Logout();

                        break;

                    case 12:

                        Login();
                        GetAllReview();
                        Logout();

                        break;

                    case 13:

                        Login();
                        EditReview();
                        Logout();

                        break;

                    case 14:

                        exit = Exit();

                        break;

                    default:

                        Console.WriteLine("Invalid choice. Please try again!.");

                        break;
                }

                Console.WriteLine("Press any key to continue....");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

}
    
