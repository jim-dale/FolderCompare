
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;

    /// <summary>
    /// Get items with same ContentsHash
    /// Excude items with same relative path
    /// Exclude items with duplicate content-hash in source
    /// </summary>
    public class MovedCommand
    {
        public CommandOption LeftPath { get; private set; }
        public CommandOption RightPath { get; private set; }
        public CommandOption<DisplayMode> Display { get; private set; }

        public MovedContext Context { get; private set; }

        public void Configure(CommandLineApplication<MovedCommand> cmd)
        {
            LeftPath = cmd.Option("-l|--left <PATH>", "The left folder or catalogue to compare.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            RightPath = cmd.Option("-r|--right <PATH>", "The right folder or catalogue to compare.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            Display = cmd.Option<DisplayMode>("-d|--displayMode <MODE>", Helpers.DisplayModeNamesAsString(), CommandOptionType.SingleValue)
                .Accepts(v => v.Enum<DisplayMode>(true));

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            Context = new MovedContext
            {
                LeftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPath.Value())),
                RightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPath.Value())),
                OutputType = DisplayMode.Same,

                EqualityComparer = new RelPathHashEqualityComparer(),
                ContentsComparer = new ContentsHashEqualityComparer(),
            };

            Context.Report = new ConsoleComparisonReport(Context.OutputType, Console.WindowWidth);

            // Get all metadata entries
            var leftItems = Context.LeftSource.GetAll();
            var rightItems = Context.RightSource.GetAll();

            // Exclude items that appear in both lists
            Context.LeftItems = leftItems.Except(rightItems, Context.EqualityComparer);
            Context.RightItems = rightItems.Except(leftItems, Context.EqualityComparer);

            // Remove items with duplicate ContentsHash value
            Context.LeftItems = Context.LeftItems.RemoveDuplicates(i => i.ContentsHash);
            Context.RightItems = Context.RightItems.RemoveDuplicates(i => i.ContentsHash);

            int combined = 0;

            var items = Context.LeftItems.FullOuterJoin(Context.RightItems, Context.ContentsComparer);
            if (items.Any())
            {
                Context.Report.SetSources(Context.LeftSource.Source, Context.RightSource.Source);

                foreach (var item in items)
                {
                    int comparison = String.Compare(item.Item1?.ContentsHash, item.Item2?.ContentsHash, StringComparison.InvariantCultureIgnoreCase);
                    combined |= comparison;

                    Context.Report.OutputRow(item.Item1, item.Item2, comparison, null);
                }
            }

            return Helpers.GetComparisonResultAsExitCode(combined);
        }

        private static void DumpDuplicates(IEnumerable<FileMetadata> items, string collectionName)
        {
            var duplicates = items.GetDuplicates(i => i.ContentsHash);

            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine($"{collectionName} has {items.Count()} items, of which there are {duplicates.Count()} duplicates");
            Console.WriteLine();

            foreach (var g in duplicates)
            {
                Console.WriteLine("+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +");
                foreach (var item in g)
                {
                    Console.WriteLine(item.RelativePath);
                }

            }
        }
    }
}



//var leftOuterJoin = from left in Context.LeftItems
//                    join right in Context.RightItems on left.ContentsHash equals right.ContentsHash into gj
//                    from subRight in gj.DefaultIfEmpty()
//                    select new
//                    {
//                        LeftItem = left,
//                        RightItem = subRight
//                    };

//foreach (var item in leftOuterJoin.Where(i => i.RightItem is null))
//{
//    Console.WriteLine($"{item.LeftItem?.RelativePath}, {item.RightItem?.RelativePath}");
//}

//Console.WriteLine();
//Console.WriteLine("+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +");
//Console.WriteLine();




//var rightOuterJoin = from right in Context.RightItems
//                    join left in Context.LeftItems on right.ContentsHash equals left.ContentsHash into gj
//                    from subLeft in gj.DefaultIfEmpty()
//                    select new
//                    {
//                        LeftItem = subLeft,
//                        RightItem = right
//                    };



//foreach (var item in rightOuterJoin.Where(i => i.LeftItem is null))
//{
//    Console.WriteLine($"{item.LeftItem?.RelativePath}, {item.RightItem?.RelativePath}");
//}

//Console.WriteLine();

//var dupLeft = Context.RightItems.GetDuplicates(i => i.ContentsHash);
//if (dupLeft.Any())
//{
//    Console.WriteLine($"{dupLeft.Count()}");
//    Console.WriteLine();

//    foreach (var g in dupLeft)
//    {
//        Console.WriteLine("+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +");
//        foreach (var item in g)
//        {
//            Console.WriteLine(item.RelativePath);
//        }
//    }
//}
//var Ljoin = from emp in ListOfEmployees
//            join proj in ListOfProject
//               on emp.ProjectID equals proj.ProjectID into JoinedEmpDept
//            from proj in JoinedEmpDept.DefaultIfEmpty()
//            select new
//            {
//                EmployeeName = emp.Name,
//                ProjectName = proj != null ? proj.ProjectName : null
//            };

////Right outer join
//var RJoin = from proj in ListOfProject
//            join employee in ListOfEmployees
//            on proj.ProjectID equals employee.ProjectID into joinDeptEmp
//            from employee in joinDeptEmp.DefaultIfEmpty()
//            select new
//            {
//                EmployeeName = employee != null ? employee.Name : null,
//                ProjectName = proj.ProjectName
//            };
