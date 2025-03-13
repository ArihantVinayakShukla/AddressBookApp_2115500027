using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Mapping
{
    public class AddressBookMapping : Profile
    {
        public AddressBookMapping()
        {
            CreateMap<AddressBookEntry, AddressBookDTO>().ReverseMap();
        }
    }
}
