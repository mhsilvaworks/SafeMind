using SafeMind.Domain;
using SafeMind.Application.Interfaces;
using SafeMind.Application.DTOs;
using SafeMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ForumService : IForumService
{
    private readonly AppDbContext _context;

    public ForumService(AppDbContext context) => _context = context;

    public async Task<Forum> CreateAsync(CreateForumDto dto, Guid ownerId)
    {
        var forum = new Forum
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            OwnerId = ownerId,
            ForumType = dto.ForumType,
            MinAge = dto.MinAge,
            RequiresVerifiedStatus = dto.RequiresVerifiedStatus
        };
        _context.Forums.Add(forum);
        await _context.SaveChangesAsync();
        return forum;
    }

    public async Task<Forum?> GetByIdAsync(Guid id) =>
        await _context.Forums.FindAsync(id);

    public async Task<IEnumerable<Forum>> GetAllAsync() =>
        await _context.Forums.ToListAsync();

    public async Task UpdateAsync(Guid forumId, UpdateForumDto dto, Guid requesterId)
    {
        var forum = await _context.Forums.FindAsync(forumId)
            ?? throw new KeyNotFoundException("Fórum não encontrado.");

        if (forum.OwnerId != requesterId)
            throw new UnauthorizedAccessException("Apenas o criador do fórum pode editá-lo.");

        forum.Title = dto.Title;
        forum.Description = dto.Description;
        forum.MinAge = dto.MinAge;
        forum.RequiresVerifiedStatus = dto.RequiresVerifiedStatus;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid forumId, Guid requesterId)
    {
        var forum = await _context.Forums.FindAsync(forumId)
            ?? throw new KeyNotFoundException("Fórum não encontrado.");

        if (forum.OwnerId != requesterId)
            throw new UnauthorizedAccessException("Apenas o criador do fórum pode excluí-lo.");

        _context.Forums.Remove(forum);
        await _context.SaveChangesAsync();
    }

    // ====================================================================
    // KAN-9: MÉTODOS DE VALIDAÇÃO E BLOQUEIO (RN01 e RN03)
    // ====================================================================

    public async Task ValidarAcessoAoForumAsync(Guid userId, Guid forumId)
    {
        var forum = await _context.Forums.FindAsync(forumId)
            ?? throw new KeyNotFoundException("Fórum não encontrado.");

        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        // RN01: VALIDAÇÃO DE IDADE MÍNIMA
        var idadeUsuario = CalcularIdade(user.BirthDate);
        if (idadeUsuario < forum.MinAge)
        {
            throw new UnauthorizedAccessException($"Acesso negado. A idade mínima para este fórum é {forum.MinAge} anos.");
        }

// RN03: VALIDAÇÃO DE LAUDO / STATUS
        if (forum.RequiresVerifiedStatus)
        {
            // Tenta tratar o usuário como alguém que tem laudo (implementa IValidavel)
            if (user is IValidavel usuarioComLaudo)
            {
                if (usuarioComLaudo.ValidationStatus != StatusLaudo.Verificado)
                {
                    throw new UnauthorizedAccessException("Acesso negado. Este fórum requer uma conta com laudo verificado.");
                }
            }
            else if (user is Administrador)
            {
                // Administradores não têm laudo, mas têm passe livre no sistema (Bypass de segurança)
            }
            else
            {
                // Qualquer outro tipo de perfil que acabe caindo aqui sem suporte a laudo
                throw new UnauthorizedAccessException("Acesso negado. O seu tipo de perfil não suporta verificação de laudos.");
            }
        }
    }
    private int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        
        // Desconta 1 ano se o usuário ainda não fez aniversário neste ano civil
        if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
        
        return idade;
    }
}