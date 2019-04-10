namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
             var prisonersToExport = context.Prisoners
                                            .Where(p => ids.Contains(p.Id))
                                            .Select(p => new
                                            {
                                                Id = p.Id,
                                                Name = p.FullName,
                                                CellNumber = p.Cell.CellNumber,
                                                Officers = p.PrisonerOfficers
                                                            .Select(po => new
                                                            {
                                                                OfficerName = po.Officer.FullName,
                                                                Department = po.Officer.Department.Name
                                                            })
                                                            .OrderBy(o => o.OfficerName)
                                                            .ToArray(),
                                                TotalOfficerSalary = p.PrisonerOfficers
                                                                      .Select(po => po.Officer.Salary)
                                                                      .Sum()
                                            })
                                            .OrderBy(p => p.Name)
                                            .ThenBy(p => p.Id)
                                            .ToArray();

            var json = JsonConvert.SerializeObject(prisonersToExport, Formatting.Indented);
            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            throw new NotImplementedException();
        }
    }
}