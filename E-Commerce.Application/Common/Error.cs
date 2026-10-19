using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public record Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string description = "General Failure description")
            => new Error(code, description, ErrorType.Failure);
        public static Error Validation(string code = "General.Validation", string description = "General Validation description")
            => new Error(code, description, ErrorType.Validation);
        public static Error NotFound(string code = "General.NotFound", string description = "General NotFound description")
            => new Error(code, description, ErrorType.NotFound);
        public static Error Conflict(string code = "General.Conflict", string description = "General Conflict description")
            => new Error(code, description, ErrorType.Conflict);
        
        public static Error Unauthorized(string code = "General.Unauthorized", string description = "General Unauthorized description")
            => new Error(code, description, ErrorType.Unauthorized);
        
        public static Error Forbidden(string code = "General.Forbidden", string description = "General Forbidden description")
            => new Error(code, description, ErrorType.Forbidden);
        
        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "General InvalidCredentials description")
            => new Error(code, description, ErrorType.InvalidCredentials);
        
    }
}
