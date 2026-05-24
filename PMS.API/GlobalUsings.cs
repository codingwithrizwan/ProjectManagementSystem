/* Here i have used global usings to avoid using the same namespaces in every file and to make the code cleaner 
This way, we can easily manage our namespaces in one place*/
global using PMS.API.Middleware;
global using PMS.Application;
global using PMS.Infrastructure.Auth;
global using PMS.API.Authorization;
global using PMS.API.Extensions;
global using PMS.Infrastructure.DependencyInjection;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using PMS.Application.DTOs;
global using MediatR;
global using PMS.Domain.Constants;
