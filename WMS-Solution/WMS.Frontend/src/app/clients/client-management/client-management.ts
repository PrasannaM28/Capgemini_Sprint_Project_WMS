import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';
import { Client } from '../../services/client';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-client-management',
  standalone: false,
  templateUrl: './client-management.html',
  styleUrl: './client-management.css',
})
export class ClientManagement implements OnInit {
  clients: any[] = [];

  loading = false;

  role = '';

  constructor(
    private clientService: Client,
    private authService: Auth,
    private router: Router,
    private feedback: UiFeedbackService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.loadClients();
  }

  get canManageClients(): boolean {
    return this.role === 'Admin';
  }

  loadClients(): void {
    this.loading = true;

    this.clientService.getAll().subscribe({
      next: (response) => {
        this.clients = response.data ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  addClient(): void {
    if (!this.canManageClients) {
      return;
    }

    this.router.navigate(['/clients/add']);
  }

  editClient(client: any): void {
    if (!this.canManageClients) {
      return;
    }

    this.router.navigate(['/clients/edit', client.clientId]);
  }

  deleteClient(id: number): void {
    if (!this.canManageClients) {
      return;
    }

    this.feedback.confirm({
      title: 'Delete client?',
      message: 'This client will be removed from the directory.',
      confirmLabel: 'Delete',
      tone: 'warning',
    }).subscribe((accepted) => {
      if (!accepted) {
        return;
      }

      this.loading = true;

      this.clientService.delete(id).subscribe({
        next: () => {
          this.feedback.success('Client deleted', 'The client was removed successfully.');
          this.loadClients();
        },
        error: () => {
          this.loading = false;
        },
      });
    });
  }
}