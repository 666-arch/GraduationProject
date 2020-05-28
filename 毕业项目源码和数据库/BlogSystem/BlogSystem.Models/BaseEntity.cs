using System;

namespace BlogSystem.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();  //编号
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public bool IsRemoved { get; set; } //是否删除 （伪删除）
    }
}
