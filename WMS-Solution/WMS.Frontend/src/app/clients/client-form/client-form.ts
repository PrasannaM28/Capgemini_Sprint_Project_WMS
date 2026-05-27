import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Client } from '../../services/client';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-client-form',
  standalone: false,
  templateUrl: './client-form.html',
  styleUrl: './client-form.css',
})
export class ClientForm implements OnInit {
  clientForm: FormGroup;

  loading = false;

  clientId = 0;

  constructor(
    private fb: FormBuilder,
    private clientService: Client,
    private route: ActivatedRoute,
    private router: Router,
    private feedback: UiFeedbackService
  ) {
    this.clientForm = this.fb.group({
      clientName: ['', Validators.required],
      clientAddress: [''],
      clientPhoneNumber: [''],
      clientLocation: [''],
    });
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (id) {
      this.clientId = id;
      this.loadClient();
    }
  }

  get isEditMode(): boolean {
    return this.clientId > 0;
  }

  get pageHeading(): string {
    return this.isEditMode ? 'Edit Client' : 'Add Client';
  }

  get f() {
    return this.clientForm.controls;
  }

  loadClient(): void {
    this.loading = true;

    this.clientService.getAll().subscribe({
      next: (response) => {
        const clients = response.data ?? [];
        const client = clients.find((item: any) => Number(item.clientId) === this.clientId);

        if (client) {
          this.clientForm.patchValue({
            clientName: client.clientName,
            clientAddress: client.clientAddress ?? '',
            clientPhoneNumber: client.clientPhoneNumber ?? '',
            clientLocation: client.clientLocation ?? '',
          });
        }

        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  saveClient(): void {
    if (this.clientForm.invalid) {
      this.clientForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    const payload = this.clientForm.value;
    const request = this.isEditMode
      ? this.clientService.update(this.clientId, payload)
      : this.clientService.create(payload);

    request.subscribe({
      next: () => {
        this.feedback.success(
          this.isEditMode ? 'Client updated' : 'Client created',
          this.isEditMode ? 'The client was updated successfully.' : 'The client was created successfully.'
        );

        this.router.navigate(['/clients']);
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/clients']);
  }
}