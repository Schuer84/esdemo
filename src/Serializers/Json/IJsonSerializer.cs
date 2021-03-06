﻿namespace SqlStreamStore.Demo.Serializers.Json
{
    public interface IJsonSerializer
    {
        object Deserialize(string json);
        T Deserialize<T>(string json);

        string Serialize(object obj);
        string Serialize<T>(T obj);
    }
}