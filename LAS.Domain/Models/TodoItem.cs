using System.Text.Json.Serialization;

namespace LAS.Domain.Models
{
    public class TodoItem
    {
        /// <summary>
        /// ID
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        [JsonPropertyName("isComplete")]
        public bool IsComplete { get; set; }

        /// <summary>
        /// 期限日
        /// </summary>
        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// 作成日
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 削除日
        /// </summary>
        [JsonPropertyName("deletedAt")]
        public DateTime? DeletedAt { get; set; }
    }
}