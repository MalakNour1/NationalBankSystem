using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using Bank_System.Contexts;
using Bank_System.Models;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        using var context = new BankDbContext();

        context.Database.Migrate();

        SeedData(context);

        bool exit = false;

        while (!exit)
        {
            Console.Clear();

            Console.WriteLine("========================================");
            Console.WriteLine("    National Bank - Management");
            Console.WriteLine("========================================");
            Console.WriteLine("  1) Add a new Customer");
            Console.WriteLine("  2) Open a new Account for a Customer");
            Console.WriteLine("  3) Update Account Status (Active / Closed)");
            Console.WriteLine("  4) Remove an Account from a Customer");
            Console.WriteLine("  5) List all Customers (with accounts)");
            Console.WriteLine("  0) Exit");
            Console.WriteLine("----------------------------------------");

            int choice = ReadInt("  Enter choice: ");

            switch (choice)
            {
                case 1:
                    AddCustomer(context);
                    break;

                case 2:
                    OpenAccount(context);
                    break;

                case 3:
                    UpdateAccountStatus(context);
                    break;

                case 4:
                    RemoveAccount(context);
                    break;

                case 5:
                    ListCustomers(context);
                    break;

                case 0:
                    exit = true;
                    context.Database.CloseConnection();
                    Console.WriteLine("Goodbye!");
                    continue;

                default:
                    PrintError("Invalid option. Please choose from 0 to 5.");
                    break;
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }
    }

    // ---------------- SEEDING ----------------

    static void SeedData(BankDbContext context)
    {
        if (!context.Branches.Any())
        {
            context.Branches.AddRange(
                new Branch
                {
                    Name = "Cairo Branch",
                    Address = "Nasr City",
                    PhoneNumber = "01012345678",
                    Manager = new Manager
                    {
                        FullName = "Ahmed Mohamed",
                        Email = "ahmed@bank.com",
                        PhoneNumber = "01098765432",
                        HireDate = new DateTime(2020, 1, 1)
                    }
                },

                new Branch
                {
                    Name = "Giza Branch",
                    Address = "Dokki",
                    PhoneNumber = "01112345678",
                    Manager = new Manager
                    {
                        FullName = "Mohamed Ali",
                        Email = "mohamed@bank.com",
                        PhoneNumber = "01198765432",
                        HireDate = new DateTime(2021, 5, 10)
                    }
                }
            );

            context.SaveChanges();
        }

        if (!context.Customers.Any())
        {
            context.Customers.AddRange(
                new Customer
                {
                    FullName = "Sara Ahmed",
                    NationalId = "30005151234567",
                    DateOfBirth = new DateTime(2000, 5, 15),
                    Email = "sara@gmail.com",
                    PhoneNumber = "01055555555",
                    Address = "Cairo",
                    CustomerType = "Individual"
                },

                new Customer
                {
                    FullName = "Omar Hassan",
                    NationalId = "29808201234567",
                    DateOfBirth = new DateTime(1998, 8, 20),
                    Email = "omar@gmail.com",
                    PhoneNumber = "01066666666",
                    Address = "Giza",
                    CustomerType = "Individual"
                }
            );

            context.SaveChanges();
        }
    }

    // ---------------- OPTION 1: ADD CUSTOMER ----------------

    static void AddCustomer(BankDbContext context)
    {
        Console.WriteLine("\n--- Add New Customer ---");

        string fullName = ReadRequired("Full Name      : ");

        string nationalId;

        while (true)
        {
            nationalId = ReadRequired("National ID (14 digits): ");

            if (nationalId.Length != 14 || !nationalId.All(char.IsDigit))
            {
                PrintError("National ID must contain exactly 14 digits.");
                continue;
            }

            if (context.Customers.Any(c => c.NationalId == nationalId))
            {
                PrintError("This National ID already exists.");
                continue;
            }

            break;
        }

        DateTime dateOfBirth = ReadDate("Date of Birth (yyyy-MM-dd): ");

        string email = ReadEmail();

        string phoneNumber = ReadPhone();

        string address = ReadRequired("Address        : ");

        string customerType = ReadMenuChoice("Customer Type:", "Individual", "Business");

        var customer = new Customer
        {
            FullName = fullName,
            NationalId = nationalId,
            DateOfBirth = dateOfBirth,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = address,
            CustomerType = customerType
        };

        context.Customers.Add(customer);

        if (TrySave(context))
            PrintSuccess($"\nCustomer created successfully. CustomerId = {customer.Id}");
    }

    // ---------------- OPTION 2: OPEN ACCOUNT ----------------

    static void OpenAccount(BankDbContext context)
    {
        Console.WriteLine("\n--- Open New Account ---");

        int accountNumber = ReadPositiveInt("Account Number : ");

        if (context.Accounts.Any(a => a.AccountNumber == accountNumber))
        {
            PrintError("This Account Number already exists.");
            return;
        }

        string accountType = ReadMenuChoice("Account Type:", "Savings", "Current", "Business");

        int branchCode = ReadPositiveInt("Branch Code    : ");
        int customerId = ReadPositiveInt("Customer Id    : ");

        string ownershipType = ReadMenuChoice("Ownership Role:", "Primary", "CoHolder");

        PrintInfo($"Validating branch '{branchCode}' and customer #{customerId}...");

        var branch = context.Branches.Find(branchCode);

        if (branch == null)
        {
            PrintError("Branch does not exist.");
            return;
        }

        var customer = context.Customers.Find(customerId);

        if (customer == null)
        {
            PrintError("Customer does not exist.");
            return;
        }

        var account = new Account
        {
            AccountNumber = accountNumber,
            AccountType = accountType,
            CurrentBalance = 0,
            OpeningDate = DateTime.Now,
            BranchId = branchCode
        };

        var customerAccount = new CustomerAccount
        {
            AccountId = accountNumber,
            CustomerId = customerId,
            OwnershipType = ownershipType,
            OwnershipStartDate = DateTime.Now,
            AccountStatus = "Active"
        };

        context.Accounts.Add(account);
        context.CustomerAccounts.Add(customerAccount);

        if (TrySave(context))
        {
            PrintSuccess($"Account '{accountNumber}' created and linked to customer {customerId} as {ownershipType} owner.");
        }
    }

    // ---------------- OPTION 3: UPDATE ACCOUNT STATUS ----------------

    static void UpdateAccountStatus(BankDbContext context)
    {
        Console.WriteLine("\n--- Update Account Status ---");

        int accountNumber = ReadPositiveInt("Account Number : ");
        int customerId = ReadPositiveInt("Customer Id    : ");

        var customerAccount = context.CustomerAccounts
            .FirstOrDefault(ca =>
                ca.AccountId == accountNumber &&
                ca.CustomerId == customerId);

        if (customerAccount == null)
        {
            PrintError("No relationship found between this Customer and Account.");
            return;
        }

        string newStatus = ReadMenuChoice("New Status:", "Active", "Closed");

        customerAccount.AccountStatus = newStatus;

        if (TrySave(context))
            PrintSuccess($"Status updated to {newStatus}.");
    }

    // ---------------- OPTION 4: REMOVE ACCOUNT FROM CUSTOMER ----------------

    static void RemoveAccount(BankDbContext context)
    {
        Console.WriteLine("\n--- Remove Account From Customer ---");

        int accountNumber = ReadPositiveInt("Account Number : ");
        int customerId = ReadPositiveInt("Customer Id    : ");

        var customerAccount = context.CustomerAccounts
            .FirstOrDefault(ca =>
                ca.AccountId == accountNumber &&
                ca.CustomerId == customerId);

        if (customerAccount == null)
        {
            PrintError("No relationship found between this Customer and Account.");
            return;
        }

        context.CustomerAccounts.Remove(customerAccount);

        bool hasOtherOwners = context.CustomerAccounts
            .Any(ca => ca.AccountId == accountNumber && ca.CustomerId != customerId);

        bool accountRemoved = false;

        if (!hasOtherOwners)
        {
            var account = context.Accounts.Find(accountNumber);

            if (account != null)
            {
                context.Accounts.Remove(account);
                accountRemoved = true;
            }
        }

        if (TrySave(context))
        {
            PrintSuccess("\n  Ownership link deleted.");

            if (accountRemoved)
                PrintInfo($"      That was the last owner - account '{accountNumber}' was also removed.");
        }
    }

    // ---------------- OPTION 5: LIST CUSTOMERS ----------------

    static void ListCustomers(BankDbContext context)
    {
        Console.WriteLine("\n--- All Customers ---\n");

        var customers = context.Customers
            .Include(c => c.Accounts)
            .ThenInclude(ca => ca.Account)
            .ToList();

        if (!customers.Any())
        {
            Console.WriteLine("No customers found.");
            return;
        }

        var branchNames = context.Branches
            .ToDictionary(b => b.Code, b => b.Name);

        foreach (var customer in customers)
        {
            Console.WriteLine($"  #{customer.Id} {customer.FullName} ({customer.CustomerType})");

            if (!customer.Accounts.Any())
            {
                Console.WriteLine("      (no accounts)");
                continue;
            }

            foreach (var ca in customer.Accounts)
            {
                var acc = ca.Account;

                branchNames.TryGetValue(acc.BranchId, out string? branchName);

                Console.WriteLine(
                    $"      {acc.AccountNumber,-8} {acc.AccountType,-9} " +
                    $"Balance: {acc.CurrentBalance,12:N2}   " +
                    $"{ca.OwnershipType,-8} {ca.AccountStatus,-7} @ {branchName}");
            }
        }
    }

    // ---------------- INPUT HELPERS ----------------

    static string ReadRequired(string message)
    {
        while (true)
        {
            Console.Write(message);

            string? value = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            PrintError("Input cannot be empty. Try again.");
        }
    }

    static int ReadInt(string message)
    {
        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out int value))
                return value;

            PrintError("Please enter a valid number.");
        }
    }

    static int ReadPositiveInt(string message)
    {
        while (true)
        {
            int value = ReadInt(message);

            if (value > 0)
                return value;

            PrintError("Number must be greater than zero.");
        }
    }

    static DateTime ReadDate(string message)
    {
        while (true)
        {
            Console.Write(message);

            if (DateTime.TryParseExact(
                    Console.ReadLine()?.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date)
                && date.Date < DateTime.Today)
            {
                return date;
            }

            PrintError("Please enter a valid date in the past (yyyy-MM-dd).");
        }
    }

    static string ReadEmail()
    {
        while (true)
        {
            string email = ReadRequired("Email          : ");

            try
            {
                var address = new MailAddress(email);

                if (address.Address == email)
                    return email;
            }
            catch (FormatException)
            {
                // Invalid email format
            }

            PrintError("Please enter a valid email address.");
        }
    }

    static string ReadPhone()
    {
        while (true)
        {
            string phone = ReadRequired("Phone (11 digits): ");

            if (phone.Length == 11 && phone.All(char.IsDigit))
                return phone;

            PrintError("Phone number must contain exactly 11 digits.");
        }
    }

    static string ReadMenuChoice(string title, params string[] options)
    {
        while (true)
        {
            Console.WriteLine(title);

            for (int i = 0; i < options.Length; i++)
                Console.WriteLine($"    {i + 1}) {options[i]}");

            int choice = ReadInt("  Choice: ");

            if (choice >= 1 && choice <= options.Length)
                return options[choice - 1];

            PrintError($"Please choose a number from 1 to {options.Length}.");
        }
    }

    // ---------------- DATABASE / OUTPUT HELPERS ----------------

    static bool TrySave(BankDbContext context)
    {
        try
        {
            context.SaveChanges();
            return true;
        }
        catch (DbUpdateException ex)
        {
            PrintError("Database error: " + (ex.InnerException?.Message ?? ex.Message));
            context.ChangeTracker.Clear();
            return false;
        }
    }

    static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}