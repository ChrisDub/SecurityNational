using System.Collections.Generic;

namespace SecurityNational_PayrollApp
{
    class TaxPercentages
    {
        /// <summary>
        /// Provides a dictionary list of the states and their corresponding tax in decimal form
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, decimal> getStateTax()
        {
            Dictionary<string, decimal> StateTax = new Dictionary<string, decimal>();

            StateTax.Add("UT", 0.05m);
            StateTax.Add("WY", 0.05m);
            StateTax.Add("NV", 0.05m);
            
            StateTax.Add("CO", 0.065m);
            StateTax.Add("ID", 0.065m);
            StateTax.Add("AZ", 0.065m);
            StateTax.Add("OR", 0.065m);
            
            StateTax.Add("WA", 0.07m);
            StateTax.Add("NM", 0.07m);
            StateTax.Add("TX", 0.07m);

            return StateTax;
        }

        /// <summary>
        /// Provides a dictionary list of the federal tax in decimal form
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, decimal> getFederalTax()
        {
            Dictionary<string, decimal> FederalTax = new Dictionary<string, decimal>();

            FederalTax.Add("Federal", 0.15m);

            return FederalTax;
        }
    }
}
