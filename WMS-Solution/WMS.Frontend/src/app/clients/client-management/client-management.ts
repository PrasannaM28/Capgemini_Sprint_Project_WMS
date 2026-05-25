import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Auth } from '../../services/auth';
import { Client } from '../../services/client';

@Component({
  selector: 'app-client-management',
  standalone: false,
  templateUrl: './client-management.html',
  styleUrl: './client-management.css',
})
export class ClientManagement implements OnInit {
  clients: any[] = [];

  clientForm: FormGroup;

  loading = false;

  editingClientId: number | null = null;

  role = '';

  constructor(
    private fb: FormBuilder,
    private clientService: Client,
    private authService: Auth
  ) {
    this.clientForm = this.fb.group({
      clientName: ['', Validators.required],
      clientAddress: [''],
      clientPhoneNumber: [''],
      clientLocation: [''],
    });
  }

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.loadClients();
  }

  get canManageClients(): boolean {
    return this.role === 'Admin';
  }

  get f() {
    return this.clientForm.controls;
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

  editClient(client: any): void {
    if (!this.canManageClients) {
      return;
    }

    this.editingClientId = client.clientId;
    this.clientForm.patchValue({
      clientName: client.clientName,
      clientAddress: client.clientAddress ?? '',
      clientPhoneNumber: client.clientPhoneNumber ?? '',
      clientLocation: client.clientLocation ?? '',
    });
  }

  cancelEdit(): void {
    this.editingClientId = null;
    this.clientForm.reset();
  }

  saveClient(): void {
    if (!this.canManageClients) {
      return;
    }

    if (this.clientForm.invalid) {
      this.clientForm.markAllAsTouched();
      return;
    }

    const payload = this.clientForm.value;
    this.loading = true;

    const request = this.editingClientId
      ? this.clientService.update(this.editingClientId, payload)
      : this.clientService.create(payload);

    request.subscribe({
      next: () => {
        this.clientForm.reset();
        this.editingClientId = null;
        this.loadClients();
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  deleteClient(id: number): void {
    if (!this.canManageClients) {
      return;
    }

    if (!confirm('Delete client?')) {
      return;
    }

    this.loading = true;

    this.clientService.delete(id).subscribe({
      next: () => this.loadClients(),
      error: () => {
        this.loading = false;
      },
    });
  }
}