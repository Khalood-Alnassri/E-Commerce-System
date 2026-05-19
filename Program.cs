using Azure;
using E_Commerce_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Linq;
using System.Text.RegularExpressions;

namespace E_Commerce_System
{
    internal class Program
    {
        static ApplicationDbContext context = new ApplicationDbContext();
        static int currentUserId = 0;

        // function to regist user (used in main class)
        static void Register()
        {
            Console.WriteLine("====REGISTER====");

            Console.Write("Enter Name: ");
            string uName = Console.ReadLine()?.Trim();

            Console.Write("Enter Email: ");
            string uEmail = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(uName) || string.IsNullOrWhiteSpace(uEmail))
            {
                Console.WriteLine("\n Name and Email are required. ");
                Console.ReadKey();
                return;
            }

            if (!Regex.IsMatch(uEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                Console.WriteLine("\nInvalid email format.");
                Console.ReadKey();
                return;
            }

            var existingUser = context.Users.FirstOrDefault(u => u.Email == uEmail);
            if (existingUser != null)
            {
                Console.WriteLine("\nEmail already exists. ");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Password: ");
            string uPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(uPassword) || uPassword.Length < 6)
            {
                Console.WriteLine("\nPassword must be at least 6 characters.");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Phone Number: ");
            string uPhone = Console.ReadLine()?.Trim();

            var user = new User
            {
                UName = uName,
                Email = uEmail,
                Password = uPassword,
                Phone = uPhone,
                Role = "User",
                CreatedAt = DateTime.Now
            };

            context.Users.Add(user);
            context.SaveChanges();

            Console.WriteLine("\nRegistration successful");
            Console.ReadKey();

        }

        // function to login in to the system (used in main class)
        static void Login()
        {
            Console.Clear();
            Console.WriteLine("===== Login =====");

            Console.Write("Enter your Email: ");
            string logEmail = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter your password: ");
            string logPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(logEmail) || string.IsNullOrWhiteSpace(logPassword))
            {
                Console.WriteLine("Email and Password are required. ");
                Console.ReadKey();
                return;
            }

            var user = context.Users.FirstOrDefault(u => u.Email == logEmail);
            if (user == null)
            {
                Console.WriteLine("Email not found.");
                Console.ReadKey();
                return;
            }

            if (user.Password != logPassword)
            {
                Console.WriteLine("Invalid password");
                Console.ReadKey();
                return;
            }

            currentUserId = user.UId;
            Console.WriteLine($"Welcome, {user.UName}");
            Console.ReadKey();
        }

       /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
        // case 1: Get user details
        public static void UserInformation()
        {
            var currentUser = context.Users.FirstOrDefault(u => u.UId == currentUserId);
            Console.WriteLine("User Info:");
            Console.WriteLine("Name: " + currentUser.UName);
            Console.WriteLine("Email: " + currentUser.Email);
            Console.WriteLine("Phone: " + currentUser.Phone);
            Console.WriteLine("Role: " + currentUser.Role);
        }

        // case 2: Add a new product
        public static void AddProduct() 
        {
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

        // case 3: Update product details
        public static void UpdateProduct()
        {
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

        // case 4:  Get a list of products
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

        // case 5: Get product details by ID
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

        // case 6:  Place a new order
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
                    Console.WriteLine("Not enough stock.");
                    continue;
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

        // case 7: Get all orders for a user
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

        // case 8: Get order details
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

        // case 9: Add a review for a product
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

                // add review
                context.Reviews.Add( new Review { Rating = rate, Comment = comment, ReviewDate = date, UId=currentUserId, PId= productId });
                context.SaveChanges(); // save change in database 
            }

            else
            {
                Console.WriteLine("Product with " + productId + " not found.");
                return;
            }

        }

        // case 10: Get all reviews for a product
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

        // case 11:  Edit review
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

        // case 12: Logout
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
                Console.WriteLine("Exit cancelled. Returning to user menu...");
                return false;
            }
        }

        // user menu
        public static void UserMenu()
        {
            bool logout = false;
            
            while (!logout)
            {
                Console.WriteLine("====================== E-Commerce System ======================");
                Console.WriteLine("1. Get user details.");
                Console.WriteLine("2. Add a new product.");
                Console.WriteLine("3. Update product details.");
                Console.WriteLine("4. Get a list of products.");
                Console.WriteLine("5. Get product details.");
                Console.WriteLine("6. Place a new order.");
                Console.WriteLine("7. Get all orders for a user.");
                Console.WriteLine("8. Get order details.");
                Console.WriteLine("9. Add a review for a product.");
                Console.WriteLine("10. Get all reviews for a product.");
                Console.WriteLine("11. Edit review");
                Console.WriteLine("12. Logout.");
                
                Console.WriteLine("Choose option you need: ");
                int option = int.Parse(Console.ReadLine()); 

                switch(option)
                {
                    case 1:

                        UserInformation();

                        break;

                    case 2:

                        AddProduct();

                        break;

                    case 3:

                        UpdateProduct();

                        break;

                    case 4:

                        ListOfProducts();

                        break;


                    case 5:

                        ProductDetail();

                        break;


                    case 6:

                        PlaceNewOrder();

                        break;


                    case 7:

                        GetUserOrders();

                        break;


                    case 8:

                        OrderDetail();

                        break;

                    case 9:

                        AddReview();

                        break;


                    case 10:

                        GetAllReview();

                        break;


                    case 11:

                        EditReview();

                        break;

                    case 12:

                        logout = Logout();
                        
                        break;

                    default:

                        Console.WriteLine("Invalid option. Please try again!.");

                        break;

                }

                Console.WriteLine("Press any key to continue....");
                Console.ReadKey();
                Console.Clear();

            }

        }

        // system main
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("===== E‑Commerce System =====");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");

                Console.Write("Choose option: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid choice.");
                    Console.ReadKey();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Register();
                        break;

                    case 2:
                        Login();

                        UserMenu();
                        break;

                    case 3:
                        exit = true;

                        break;

                    default:
                        Console.WriteLine("Invalid choice");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

}
    
