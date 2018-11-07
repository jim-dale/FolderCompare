namespace FolderCompare
{
    public class ContentsCommand
    {
        // Get items with same ContentsHash
        // Excude items with same relative path
        // Exclude items with duplicate content-hash in source





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

    }
}
