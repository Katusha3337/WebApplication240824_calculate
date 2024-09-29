using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using NCalc;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var expression = context.Request.Query["expression"].ToString();
    string result = string.Empty;

    if (!string.IsNullOrEmpty(expression))
    {
        try
        {
            result = EvaluateExpression(expression).ToString();
        }
        catch
        {
            result = "Ошибка в выражении.";
        }
    }

    await context.Response.WriteAsync($@"
        <form method='get'>
            <input type='text' name='expression' placeholder='Введите математическое выражение' value='{expression}' />
            <button type='submit'>Вычислить</button>
        </form>
        <p>Результат: {result}</p>
    ", Encoding.UTF8);
});

app.Run();

double EvaluateExpression(string expression)
{
    var e = new Expression(expression);
    e.EvaluateFunction += (name, args) =>
    {
        if (name == "Pow")
        {
            var baseValue = Convert.ToDouble(args.Parameters[0].Evaluate());
            var exponent = Convert.ToDouble(args.Parameters[1].Evaluate());
            args.Result = Math.Pow(baseValue, exponent);
        }
    };
    return Convert.ToDouble(e.Evaluate());
}