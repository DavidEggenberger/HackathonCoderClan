using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data.Entities;

namespace WebAPI.GitHub
{
    public class GitHubRootProfile : Profile
    {
        public GitHubRootProfile()
        {
            CreateMap<GitHubRoot, GitHubRootEntity>()
                .ForMember(s => s.Id, opt => opt.Ignore())
                .ForMember(s => s.ApplicationUser, opt => opt.Ignore());
        }
    }
}
