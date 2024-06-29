namespace LAS.Domain.Models
{
    public class TodoItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// 期限日
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// 作成日
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 削除日
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
