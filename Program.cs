using FluentNHibernateExample.Models;
using Microsoft.Identity.Client;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NHibernate.Linq;

namespace FluentNHibernateExample
{
    class Program
    {
        static List<Student> studentList = new List<Student>();
        static List<ClassRoom> classRoomList = new List<ClassRoom>();
        static List<Teacher> teacherList = new List<Teacher>();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Quan ly sinh vien");
                Console.WriteLine("1. Xem danh sach sinh vien");
                Console.WriteLine("2. Them moi sinh vien");
                Console.WriteLine("3. Chinh sua sinh vien");
                Console.WriteLine("4. Xoa sinh vien");
                Console.WriteLine("5. Sap xep sinh vien theo ten");
                Console.WriteLine("6. Tim kiem sinh vien theo ID");
                Console.WriteLine("0. Thoat");
                Console.Write("Vui long chon: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        XemDanhSach();
                        break;
                    case "2":
                        ThemMoi();
                        break;
                    case "3":
                        SuaSinhVien();
                        break;
                    case "4":
                        XoaSinhVien();
                        break;
                    case "5":
                        SapXepTen();
                        break;
                    case "6":
                        TimTheoID();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Chon sai! Nhan phim bat ky de tiep tuc."); Console.ReadKey();
                        break;
                }
                Console.WriteLine("\nNhan phim bat ky de tiep tuc...");
                Console.ReadKey();
            }
        }

        public static void XemDanhSach()
            {
            using (var session = NHibernateHelper.GetSession())
                {
                //C
                var studentList = session.Query<Student>().ToList();
                
                if (!studentList.Any())
                {
                    Console.WriteLine("Khong co sinh vien nao trong DB");
                    return;
                }

                foreach (var student in studentList)
                {
                    Console.WriteLine($"{student.ID,-5} | {student.Name,-30} | {student.Birthday.ToShortDateString(),-12} | {student.Address,-50} | {student.ClassRoom, -10}");
                }
            }
        }

        public static void ThemMoi()
        {
            using (var session = NHibernateHelper.GetSession())
            {
                Console.WriteLine("Nhập tên sinh viên: ");
                string name = Console.ReadLine();

                DateTime birthday;
                Console.Write("Nhập ngày sinh (dd/MM/yyyy): ");
                while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out birthday))
                {
                    Console.WriteLine("Ngày sinh không hợp lệ. Vui lòng nhập đúng định dạng dd/MM/yyyy:");
                    Console.Write("Nhập ngày sinh (dd/MM/yyyy): ");
                }



                Console.WriteLine("Nhap dia chi: ");
                string address = Console.ReadLine();



                Console.WriteLine("Nhap ID lop hoc:");
                string classRoomIdInput = Console.ReadLine();

                int classRoomID;
                ClassRoom selectedClassRoom = null;

                if (int.TryParse(classRoomIdInput, out classRoomID))
                {
                    selectedClassRoom = session.Get<ClassRoom>(classRoomID);

                    if (selectedClassRoom == null)
                    {
                        Console.WriteLine($"Không tìm thấy lớp học với ID = {classRoomID}.");
                    }
                }
                else
                {
                    Console.WriteLine("ID lớp học không hợp lệ. Vui lòng nhập một số nguyên.");
                }

                var student = new Student
                {
                    Name = name,
                    Birthday = birthday,
                    Address = address,
                    ClassRoom = selectedClassRoom
                };
                session.Save(student);
            }
        }

        public static void SuaSinhVien()
        {
            using (var session = NHibernateHelper.GetSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        Console.WriteLine("\n--- CHỈNH SỬA THÔNG TIN SINH VIÊN ---");
                        Console.Write("Nhap ID sinh vien can sua: ");
                        string studentIdInput = Console.ReadLine();

                        int studentID;
                        if (!int.TryParse(studentIdInput, out studentID))
                        {
                            Console.WriteLine("ID sinh viên không hợp lệ. Vui lòng nhập một số nguyên.");
                            transaction.Rollback(); 
                            return; 
                        }

                        Student studentToUpdate = session.Get<Student>(studentID);

                        if (studentToUpdate == null)
                        {
                            Console.WriteLine($"Không tìm thấy sinh viên với ID = {studentID}.");
                            transaction.Rollback(); 
                            return; 
                        }

                        Console.WriteLine($"\n--- Đang chỉnh sửa sinh viên: {studentToUpdate.Name} (ID: {studentToUpdate.ID}) ---");
                        Console.WriteLine("De giu nguyen thong tin cu hay de trong va nhan Enter.");

                        Console.Write($"Nhập Tên mới ({studentToUpdate.Name}): ");
                        string newName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newName)) 
                        {
                            studentToUpdate.Name = newName;
                        }

                        Console.Write($"Nhập Ngày sinh mới (dd/MM/yyyy) ({studentToUpdate.Birthday.ToShortDateString()}): ");
                        string dobInput = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(dobInput))
                        {
                            DateTime newBirthday;
                            if (DateTime.TryParseExact(dobInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out newBirthday))
                            {
                                studentToUpdate.Birthday = newBirthday;
                            }
                            else
                            {
                                Console.WriteLine("Ngày sinh mới không hợp lệ. Giữ nguyên ngày sinh cũ.");
                            }
                        }

                        Console.Write($"Nhập Địa chỉ mới ({studentToUpdate.Address}): ");
                        string newAddress = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newAddress))
                        {
                            studentToUpdate.Address = newAddress;
                        }

                        ClassRoom newClassRoom = null;
                        Console.Write($"Nhập Mã số Lớp học mới (hiện tại: {studentToUpdate.ClassRoom?.Name ?? "Chưa gán"}) hoặc để trống: ");
                        string newClassRoomIdInput = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newClassRoomIdInput))
                        {
                            if (int.TryParse(newClassRoomIdInput, out int newClassRoomId))
                            {
                                newClassRoom = session.Get<ClassRoom>(newClassRoomId);
                                if (newClassRoom != null)
                                {
                                    studentToUpdate.ClassRoom = newClassRoom;
                                }
                                else
                                {
                                    Console.WriteLine($"Không tìm thấy lớp học với ID = {newClassRoomId}. Giữ nguyên lớp học cũ.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Mã số lớp học mới không hợp lệ. Giữ nguyên lớp học cũ.");
                            }
                        }

                        transaction.Commit(); 
                        Console.WriteLine("Cập nhật thông tin sinh viên thành công!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); 
                        Console.WriteLine($"Đã xảy ra lỗi khi chỉnh sửa sinh viên: {ex.Message}");
                    }
                }
            }
        }

        public static void XoaSinhVien()
        {
            using (var session = NHibernateHelper.GetSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    Console.WriteLine("Nhập ID sinh viên cần xóa:");
                    string studentIdInput = Console.ReadLine(); 

                    int studentID; 
                    Student deleting_student = null; 

                    if (int.TryParse(studentIdInput, out studentID))
                    {
                        deleting_student = session.Get<Student>(studentID);

                        if (deleting_student == null)
                        {
                            Console.WriteLine($"Không tìm thấy sinh viên với ID = {studentID}.");
                            transaction.Rollback(); 
                            return;
                        }

                        Console.Write($"Bạn có chắc chắn muốn xóa sinh viên '{deleting_student.Name}' (ID: {deleting_student.ID})? (y/n): ");
                        string confirmation = Console.ReadLine();

                        if (confirmation.ToLower() == "y")
                        {
                            session.Delete(deleting_student); 
                            transaction.Commit(); 
                            Console.WriteLine("Xóa sinh viên thành công!");
                        }
                        else
                        {
                            Console.WriteLine("Hủy bỏ thao tác xóa.");
                            transaction.Rollback(); 
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID sinh viên không hợp lệ. Vui lòng nhập một số nguyên.");
                        transaction.Rollback(); 
                    }
                }
            }
        }

        public static void TimTheoID()
        {
            using (var session = NHibernateHelper.GetSession())
            {
                try
                {
                    Console.WriteLine("\n--- TIM KIEM SINH VIEN THEO ID ---");
                    Console.Write("Nhap ID sinh vien can tim: ");
                    string studentIdInput = Console.ReadLine();

                    int studentID;
                    if (!int.TryParse(studentIdInput, out studentID))
                    {
                        Console.WriteLine("ID sinh viên không hợp lệ. Vui lòng nhập một số nguyên.");
                        return; 
                    }

                    Student foundStudent = session.Get<Student>(studentID);

                    if (foundStudent == null)
                    {
                        Console.WriteLine($"Khong tim thay sinh vien voi ID = {studentID}.");
                    }
                    else
                    {
                        Console.WriteLine("\n--- THONG TIN SINH VIEN ---");
                        Console.WriteLine($"ID: {foundStudent.ID}");
                        Console.WriteLine($"Ten: {foundStudent.Name}");
                        Console.WriteLine($"Ngay sinh: {foundStudent.Birthday.ToShortDateString()}");
                        Console.WriteLine($"Đia chi: {foundStudent.Address}");

                        if (foundStudent.ClassRoom != null)
                        {
                            Console.WriteLine($"Lop hoc: {foundStudent.ClassRoom.Name} (ID: {foundStudent.ClassRoom.ID})");
                            Console.WriteLine($"Mon hoc lop: {foundStudent.ClassRoom.Subject}");
                            if (foundStudent.ClassRoom.Teacher != null)
                            {
                                Console.WriteLine($"GVCN lop: {foundStudent.ClassRoom.Teacher.Name} (ID: {foundStudent.ClassRoom.Teacher.ID})");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Lop hoc: Chua đuoc xep.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Đã xảy ra lỗi khi tìm kiếm sinh viên: {ex.Message}");
                }
            }
        }

        public static void SapXepTen()
        {
            Console.WriteLine("Dang tai danh sach sinh vien da sap xep...");
            using (var session = NHibernateHelper.GetSession())
            {
                try
                {
                    var studentList = session.Query<Student>()
                                            .OrderBy(s => s.Name.ToLower())
                                            .ToList();

                    if (!studentList.Any())
                    {
                        Console.WriteLine("Khong co sinh vien nao trong DB.");
                        return;
                    }

                    foreach (var student in studentList)
                    {
                        string className = student.ClassRoom != null ? student.ClassRoom.Name : "N/A";

                        Console.WriteLine($"{student.ID,-5} | {student.Name,-30} | {student.Birthday.ToShortDateString(),-12} | {student.Address,-50} | {className,-15}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Da xay ra loi khi lay danh sach sinh vien: {ex.Message}");
                }
            }
        }
    }
}