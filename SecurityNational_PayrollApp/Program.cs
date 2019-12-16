using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SecurityNational_PayrollApp
{
    class Program
    {
        #region Attributes
        /// <summary>
        /// Allows an easy switch to turn on and off debug methods.
        /// </summary>
        public static int debug = 0;

        /// <summary>
        /// Object that the Employee data is imported into.
        /// </summary>
        public static List<Employee> Employees = new List<Employee>();

        /// <summary>
        /// Object to keep track of the different states and their tax percentages (in decimal form).
        /// </summary>
        public static Dictionary<string, decimal> StateTaxDictionary = new Dictionary<string, decimal>();

        /// <summary>
        /// Object to keep track of the federal tax percentage - decided to keep it the same way as state, 
        /// not really needed as a dictionary.
        /// </summary>
        public static Dictionary<string, decimal> FederalTaxDictionary = new Dictionary<string, decimal>();

        /// <summary>
        /// The resources path used to locate the file to populate the Employee List.
        /// </summary>
        public static string ResourcesPath = "..\\..\\Resources\\";

        /// <summary>
        /// The reports path used to save the generated reports into the proper location.
        /// </summary>
        public static string ReportsPath = "..\\..\\Reports\\";

        
        /*Temp variables used for manipulating and inserting data into reports.*/
        public static string EmployeeId = "";
        public static string FirstName = "";
        public static string LastName = "";
        public static char PayType = ' ';
        public static decimal Salary = 0.00m;
        public static DateTime StartDate;
        public static string State = "";
        public static int HoursWorked = 0;
        public static decimal GrossPay = 0.00m;
        public static decimal FederalTax = 0.00m;
        public static decimal StateTax = 0.00m;
        public static decimal NetPay = 0.00m;
        public static int YearsOfService = 0;

        #endregion

        static void Main(string[] args)
        {
            string EmployeeData;

            if (debug == 1)
            {
                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();
                Employees = PopulateEmployeeList(ResourcesPath, "Employees.txt");
                watch.Stop();
                Console.WriteLine($"Execution Time of populating the employee list: {watch.ElapsedMilliseconds} ms");
                watch.Reset();

                watch.Start();
                Generate_1_PayrollData();
                watch.Stop();
                Console.WriteLine($"Execution Time of generating 1 payroll report: {watch.ElapsedMilliseconds} ms");
                watch.Reset();

                watch.Start();
                Generate_2_Top15();
                watch.Stop();
                Console.WriteLine($"Execution Time of generating 2 top 15 report: {watch.ElapsedMilliseconds} ms");
                watch.Reset();

                watch.Start();
                Generate_3_StateMedianReport();
                watch.Stop();
                Console.WriteLine($"Execution Time of generating state median report: {watch.ElapsedMilliseconds} ms");
                watch.Reset();

                watch.Start();
                EmployeeData = GetByEmployeeId("1");
                watch.Stop();
                Console.WriteLine($"Execution Time of access employee by employeeID: {watch.ElapsedMilliseconds} ms");
                Console.WriteLine(EmployeeData);

            }
            else
            {

                Employees = PopulateEmployeeList(ResourcesPath, "Employees.txt");
                Console.WriteLine("Completed populating the employee list.");

                Generate_1_PayrollData();
                Console.WriteLine("Completed generating the payroll report.");

                Generate_2_Top15();
                Console.WriteLine("Completed generating the top 15% report.");

                Generate_3_StateMedianReport();
                Console.WriteLine("Completed generating the state median report.");

                EmployeeData = GetByEmployeeId("1");
                Console.WriteLine(EmployeeData);
            }

            Console.ReadLine();
        }

        #region Requested Method
        /// <summary>
        /// Provides the employee data that is associated with the passed in EmployeeId
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        private static string GetByEmployeeId(string EmployeeId)
        {
            var query = (from e in Employees select e).Where(e => e.EmployeeId.Contains(EmployeeId)).ToList();

            EmployeeId = query[0].EmployeeId;
            FirstName = query[0].FirstName;
            LastName = query[0].LastName;
            PayType = query[0].PayType;
            Salary = query[0].Salary;
            StartDate = query[0].StartDate;
            string strStartDate = StartDate.ToString("MM/DD/YYYY");
            State = query[0].State;
            HoursWorked = query[0].HoursWorked;

            return (EmployeeId + ", " + FirstName + ", " + LastName + ", " + PayType + ", " + Salary +
                ", " + strStartDate + ", " + State + ", " + HoursWorked);
        }

        #endregion

        #region Report Methods
        /// <summary>
        /// Generates a report of descending Gross Pay (highest to lowest) records that displays their payroll information.
        /// </summary>
        private static void Generate_1_PayrollData()
        {
            var orderByGrossPayDecending =
               from e in Employees
               orderby e.GrossPay descending
               select e;

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(ReportsPath, "1_PayrollData.txt")))
            {
                foreach (Employee val in orderByGrossPayDecending)
                {
                    EmployeeId = val.EmployeeId;
                    FirstName = val.FirstName;
                    LastName = val.LastName;
                    GrossPay = CalculateGrossPay(val.PayType, val.Salary, val.HoursWorked);
                    FederalTax = CalculateTax(GrossPay, FederalTaxDictionary["Federal"]);
                    StateTax = CalculateTax(GrossPay, StateTaxDictionary[val.State]);
                    NetPay = GrossPay - FederalTax - StateTax;

                    outputFile.WriteLine(EmployeeId + ", " + FirstName + ", " + LastName + ", " + GrossPay + ", " +
                    FederalTax + ", " + StateTax + ", " + NetPay);
                }
            }
        }

        /// <summary>
        /// Generates the top 15 percent of the highest Gross payed employees. Provides a list of Employee's along with their
        /// years of service and gross pay.
        /// </summary>
        private static void Generate_2_Top15()
        {
            var Top15 = (from e in Employees
                         orderby e.GrossPay, e.LastName, e.FirstName descending
                         select e).Take(150000);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(ReportsPath, "2_Top15Percent.txt")))
            {
                foreach (Employee val in Top15)
                {
                    FirstName = val.FirstName;
                    LastName = val.LastName;
                    YearsOfService = val.YearsOfService;
                    GrossPay = CalculateGrossPay(val.PayType, val.Salary, val.HoursWorked);

                    outputFile.WriteLine(FirstName + ", " + LastName + ", " + YearsOfService + ", " + GrossPay);
                }
            }
        }

        /// <summary>
        /// Generates a report of all of the listed states in the Employee Dataset along with the median hours worked, 
        /// net pay, and state tax of each. 
        /// </summary>
        private static void Generate_3_StateMedianReport()
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(ReportsPath, "3_StateMedianReport.txt")))
            {
                foreach (KeyValuePair<string, decimal> entry in StateTaxDictionary)
                {
                    DetermineMedianDataSet(entry.Key);

                    outputFile.WriteLine(State + ", " + HoursWorked + ", " + NetPay + ", " + StateTax);
                }
            }
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Helps calculate the tax with the passed in gross pay along with the tax percent in decimal form. Also ensures
        /// the returned value is rounded to 2 decimal places.
        /// </summary>
        /// <param name="grossPay"></param>
        /// <param name="tax"></param>
        /// <returns></returns>
        private static decimal CalculateTax(decimal grossPay, decimal tax)
        {
            return Round(grossPay * tax);
        }

        /// <summary>
        /// Helps calculate the gross pay depending on if the employee is hourly or salary. Built in logic to determine 
        /// overtime for hourly employees (between 80 - 90 hrs an additional 150% is added, anything above 90 an 
        /// additional 175% is added.)
        /// </summary>
        /// <param name="payType"></param>
        /// <param name="salary"></param>
        /// <param name="hoursWorked"></param>
        /// <returns></returns>
        private static decimal CalculateGrossPay(char payType, decimal salary, int hoursWorked)
        {
            decimal GrossPay = 0.00m;
            if(char.ToLower(payType) == 'h')
            {
                if(hoursWorked <= 80)
                {
                    GrossPay = salary * hoursWorked;
                }

                if (hoursWorked > 80 && hoursWorked <= 90)
                {
                    GrossPay = salary * hoursWorked * (decimal)1.5;
                }

                if (hoursWorked > 90)
                {
                    GrossPay = salary * hoursWorked * (decimal)1.75;
                }
            }

            if (char.ToLower(payType) == 's')
            {
                GrossPay = salary;
            }

            return Round(GrossPay);
        }

        /// <summary>
        /// Helps populate the list object from the provided file.
        /// </summary>
        /// <returns></returns>
        private static List<Employee> PopulateEmployeeList(string ResourcePath, string FileName)
        {
            string path = Path.Combine(ResourcePath, FileName);

            List<Employee> myList = new List<Employee>();
            if (debug == 1)
            {
                Console.WriteLine(path);
            }

            int row = 1;

            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (debug == 1)
                    {
                        Console.WriteLine("in while");
                    }

                    string[] lineData = line.Split(',');

                    ValidateData(row, ref EmployeeId, ref FirstName, ref LastName, ref PayType,
                        ref Salary, ref StartDate, ref State, ref HoursWorked, lineData);

                    StateTaxDictionary = TaxPercentages.getStateTax();
                    FederalTaxDictionary = TaxPercentages.getFederalTax();

                    GrossPay = CalculateGrossPay(PayType, Salary, HoursWorked);
                    FederalTax = CalculateTax(GrossPay, FederalTaxDictionary["Federal"]);
                    StateTax = CalculateTax(GrossPay, StateTaxDictionary[State]);
                    NetPay = CalcluateNetPay(GrossPay, FederalTax, StateTax);
                    YearsOfService = CalculateYearsOfService(StartDate);

                    Employee ee = new Employee(EmployeeId, FirstName, LastName, PayType, Salary,
                         StartDate, State, HoursWorked, GrossPay, FederalTax, StateTax, NetPay, YearsOfService);
                    
                    myList.Add(ee);
                     
                    if (debug == 1)
                    {
                         Console.WriteLine(ee.ToString());
                    }
                    row = row + 1;
                }

            }

            return myList;
        }

        /// <summary>
        /// Helps calculates the years of service with the passed in start date.
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        private static int CalculateYearsOfService(DateTime startDate)
        {
            DateTime currentDate = DateTime.Today;
            return currentDate.Year - startDate.Year;
        }

        /// <summary>
        /// Helps calculate the net pay, which is the gross pay with the federal and state taxes deducted.
        /// </summary>
        /// <param name="grossPay"></param>
        /// <param name="federalTax"></param>
        /// <param name="stateTax"></param>
        /// <returns></returns>
        private static decimal CalcluateNetPay(decimal grossPay, decimal federalTax, decimal stateTax)
        {
            return Round(grossPay - federalTax - stateTax);
        }

        /// <summary>
        /// Helps round the passed in value to 2 decimal places.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal Round(decimal val)
        {
            return Math.Round(val, 2);
        }

        /// <summary>
        /// Helps determine the median data with the passed in state.
        /// </summary>
        /// <param name="state"></param>
        private static void DetermineMedianDataSet(string state)
        {
            int count = (from e in Employees select e).Where(e => e.State.Contains(state)).Count();
            count = count / 2;

            var query = (from e in Employees select e).Where(e => e.State.Contains(state)).ToList();

            State = query[count].State;
            HoursWorked = query[count].HoursWorked;
            NetPay = query[count].NetPay;
            StateTax = StateTaxDictionary[state];
        
}
        #endregion

        #region Validation Methods
        private static void ValidateData(int row, ref string employeeId, ref string firstName, 
            ref string lastName, ref char payType, ref decimal salary, ref DateTime startDate, 
            ref string state, ref int hoursWorked, string[] lineData)
        {
            try
            {
                employeeId = lineData[0];
                firstName = lineData[1];
                lastName = lineData[2];
                payType = char.Parse(lineData[3]);
                salary = decimal.Parse(lineData[4]);
                StartDate = Convert.ToDateTime(lineData[5]);
                state = lineData[6];
                hoursWorked = int.Parse(lineData[7]);

            }
            catch (Exception ex)
            {
                Console.WriteLine("The input value was invalid on line " + row);
                if (debug == 1)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        #endregion
    }
}
