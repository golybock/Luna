﻿using Luna.Models.Tasks.Database.CardAttributes;
using Npgsql;
using Npgsql.Extension.Options;
using Npgsql.Extension.Repositories;

namespace Luna.Tasks.Repositories.Repositories.CardAttributes.Type;

public class TypeRepository : NpgsqlRepository, ITypeRepository
{

	public TypeRepository(IDatabaseOptions databaseOptions) : base(databaseOptions) { }

	public async Task<IEnumerable<TypeDatabase>> GetTypesAsync(Guid workspaceId)
	{
		var query = "SELECT * FROM type WHERE workspace_id = $1";

		var parameters = new NpgsqlParameter[]
		{
			new NpgsqlParameter() {Value = workspaceId}
		};

		return await GetListAsync<TypeDatabase>(query, parameters);
	}

	public async Task<TypeDatabase?> GetTypeAsync(Guid workspaceId, Guid typeId)
	{
		var query = "SELECT * FROM type WHERE id = $1 AND workspace_id = $2";

		var parameters = new NpgsqlParameter[]
		{
			new NpgsqlParameter() {Value = typeId},
			new NpgsqlParameter() {Value = workspaceId}
		};

		return await GetAsync<TypeDatabase>(query, parameters);
	}

	public async Task<TypeDatabase?> GetTypeAsync(Guid id)
	{
		var query = "SELECT * FROM type WHERE id = $1";

		var parameters = new NpgsqlParameter[]
		{
			new NpgsqlParameter() {Value = id}
		};

		return await GetAsync<TypeDatabase>(query, parameters);
	}

	public async Task<IEnumerable<TypeDatabase>> GetTypeAsync(IEnumerable<Guid> ids)
	{
		var query = "SELECT * FROM type WHERE id = any ($1)";

		var parameters = new NpgsqlParameter[]
		{
			new NpgsqlParameter() {Value = ids.ToArray()}
		};

		return await GetListAsync<TypeDatabase>(query, parameters);
	}

	public async Task<Boolean> CreateTypeAsync(TypeDatabase type)
	{
		var query = "INSERT INTO type (id, name, hex_color, workspace_id, deleted) VALUES ($1, $2, $3, $4, $5)";

		var parameters = new NpgsqlParameter[]
		{
			new NpgsqlParameter() {Value = type.Id},
			new NpgsqlParameter() {Value = type.Name},
			new NpgsqlParameter() {Value = type.HexColor},
			new NpgsqlParameter() {Value = type.WorkspaceId},
			new NpgsqlParameter() {Value = type.Deleted}
		};

		return await ExecuteAsync(query, parameters);
	}

	public async Task<Boolean> UpdateTypeAsync(Guid id, TypeDatabase type)
	{
		var query = "UPDATE type SET name = $2, hex_color = $3, deleted = $4 WHERE id = $1";

		var parameters = new NpgsqlParameter[]
		{
			new NpgsqlParameter() {Value = id},
			new NpgsqlParameter() {Value = type.Name},
			new NpgsqlParameter() {Value = type.HexColor},
			new NpgsqlParameter() {Value = type.Deleted},
		};

		return await ExecuteAsync(query, parameters);
	}

	public Task<Boolean> DeleteTypeAsync(Guid id)
	{
		return DeleteAsync("type", nameof(id), id);
	}

	public async Task<bool> TrashTypeAsync(Guid id)
	{
		var query = "UPDATE type deleted = true WHERE id = $1";

		var parameters = new NpgsqlParameter[]
		{
			new NpgsqlParameter() {Value = id},
		};

		return await ExecuteAsync(query, parameters);
	}
}