using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.Authentication;
using Fun_Funding.Application.ViewModel.AuthenticationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IAuthenticationService
    {
        Task<ResultDTO<string>> RegisterUserAsync(RegisterRequest registerModel, IList<string> roles);
        Task<ResultDTO<string>> LoginAsync(LoginRequest loginDTO);
    }
}
