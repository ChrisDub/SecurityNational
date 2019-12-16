using System;
using System.Reflection;

namespace SecurityNational_PayrollApp
{
    class Employee
    {
        /// <summary>
        /// The employee's identifier
        /// </summary>
        public string EmployeeId; //look into having this be a char(7)

        /// <summary>
        /// The employee's first name
        /// </summary>
        public string FirstName;

        /// <summary>
        /// The employee's last name
        /// </summary>
        public string LastName;

        /// <summary>
        /// Pay type is either Hourly decoded as: H, or Salary decoded as: S
        /// </summary>
        public char PayType;

        /// <summary>
        /// If the employee is hourly this amount is their hourly rate, 
        ///     if they are salary this is their annual salary amount.
        /// </summary>
        public decimal Salary;

        /// <summary>
        /// The start date the employee's employment.
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// The state the employee works in.
        /// </summary>
        public string State;

        /// <summary>
        /// The hours worked in a two week period.
        /// </summary>
        public readonly int HoursWorked;

        /// <summary>
        /// The amount the Employee takes home without taxes taken out.
        /// </summary>
        public decimal GrossPay;

        /// <summary>
        /// The amount of federal tax taken out of the Employee's Net Pay
        /// </summary>
        public decimal FederalTax;

        /// <summary>
        /// The amount of state tax taken out of the Employee's Net Pay
        /// </summary>
        public decimal StateTax;

        /// <summary>
        /// The amount of the Employee's taken home with taxes taken out
        /// </summary>
        public decimal NetPay;

        /// <summary>
        /// The number of years the Employee has been working for the company.
        /// </summary>
        public int YearsOfService;

        /// <summary>
        /// Method to construct an employee along with their attributes 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="payType"></param>
        /// <param name="salary"></param>
        /// <param name="startDate"></param>
        /// <param name="state"></param>
        /// <param name="hoursWorked"></param>
        /// <param name="grossPay"></param>
        /// <param name="federalTax"></param>
        /// <param name="stateTax"></param>
        /// <param name="netPay"></param>
        /// <param name="yearsOfService"></param>
        public Employee(string employeeId, string firstName, string lastName, char payType, decimal salary, DateTime startDate,
            string state, int hoursWorked, decimal grossPay, decimal federalTax, decimal stateTax, decimal netPay, int yearsOfService)
        {
            try
            {
                this.EmployeeId = employeeId;
                this.FirstName = firstName;
                this.LastName = lastName;
                this.PayType = payType;
                this.Salary = salary;
                this.StartDate = startDate;
                this.State = state;
                this.HoursWorked = hoursWorked;
                this.GrossPay = grossPay;
                this.FederalTax = federalTax;
                this.StateTax = stateTax;
                this.NetPay = netPay;
                this.YearsOfService = yearsOfService;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "."
                + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// To string method to verify the employee's attributes were assigned correctly. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}", 
                EmployeeId, FirstName, LastName, PayType, Salary, StartDate, State, HoursWorked, GrossPay, FederalTax,
                StateTax, NetPay, YearsOfService);
        }
    }

}
