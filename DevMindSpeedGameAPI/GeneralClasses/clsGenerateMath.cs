namespace DevMindSpeedGameAPI.General
{
    static public class clsGenerateMath
    {
        static public (string Question, float Answer) GenerateMathQuestion(int difficulty)
        {
            var random = new Random();
            var operations = new[] { "+", "-", "*", "/" };

            int operandCount = difficulty + 1;
            double maxDigit = Math.Pow(10, difficulty);

            var numbers = Enumerable.Range(0, operandCount)
                                    .Select(_ => random.Next(1, (int)maxDigit))
                                    .Select(num => (double)num)
                                    .ToArray();
            var operation = operations[random.Next(operations.Length)];

            string question = string.Join($" {operation} ", numbers);
            var result = new System.Data.DataTable().Compute(question, null);
            float answer = Convert.ToSingle(result);

            return (question, answer);
        }

    }
}
