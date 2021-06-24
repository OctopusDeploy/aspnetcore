namespace Microsoft.AspNetCore.Routing
{
    internal interface ILiteralConstraint : IParameterPolicy
    {
        bool MatchLiteral(string parameterName, string literal);
    }
}
