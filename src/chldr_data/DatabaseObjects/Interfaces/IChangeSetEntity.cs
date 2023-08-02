﻿namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IChangeSetEntity : IChangeSet, IEntity
    {
        public int RecordType { get; set; }
        public int Operation { get; set; }
    }
}