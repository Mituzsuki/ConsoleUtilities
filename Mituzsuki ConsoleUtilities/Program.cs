//Mocking up example objects
//The fact the ID's won't match doesn't really matter just a POC as none of the printing relies on it

using Mituzsuki.ConsoleUtilities;

Department department = new Department();

Subject warhammerSubject = new Subject() {
    Name = "Warhammer40k Lore",
    DepartmentId = department.DepartmentId,
    Department = department
};

Subject skyrimSubject = new Subject() {
    Name = "Skyrime Lore",
    DepartmentId = department.DepartmentId,
    Department = department
};

Student exampleStudent = new Student() {
    Subjects = new List<Subject>(){
        warhammerSubject,
        skyrimSubject
    }
};

Util util = new Util();

Console.WriteLine(util.GetPropertyTreeAsString(exampleStudent));

exampleStudent.Subjects[0].Department = null;
exampleStudent.Name = null;

Console.WriteLine(util.GetPropertyTreeAsString(exampleStudent));
