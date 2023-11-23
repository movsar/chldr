﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.sql.Services;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.sql.SqlEntities;
[Table("Sound")]

public class SqlSound : ISoundEntity
{
    [Key]
    public string SoundId { get; set; }
    public string UserId { get; set; } = null!;
    public string EntryId { get; set; } = null!;
    public int Rate { get; set; }
    public string FileName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public virtual SqlEntry Entry { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;


    internal static SqlSound FromDto(PronunciationDto soundDto, SqlContext context)
    {
        //var user = context.Find<SqlUser>(soundDto.UserId);

        var soundEntity = context.Find<SqlSound>(soundDto.SoundId);
        if (soundEntity == null)
        {
            soundEntity = new SqlSound();
        }

        soundEntity.EntryId = soundDto.EntryId;
        soundEntity.UserId = soundDto.UserId;
        soundEntity.SoundId = soundDto.SoundId;
        soundEntity.FileName = soundDto.FileName;
        soundEntity.Rate = soundDto.Rate;

        return soundEntity;
    }
}