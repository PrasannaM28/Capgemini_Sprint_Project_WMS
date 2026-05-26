import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { FeedbackConfirmState, FeedbackToast, UiFeedbackService } from './ui-feedback.service';

@Component({
	selector: 'app-feedback-host',
	standalone: false,
	templateUrl: './ui-feedback-host.html',
	styleUrl: './ui-feedback-host.css',
})
export class UiFeedbackHost {
	@Input() shellMode = false;

	readonly toasts$: Observable<FeedbackToast[]>;
	readonly confirm$: Observable<FeedbackConfirmState | null>;

	constructor(private feedback: UiFeedbackService) {
		this.toasts$ = this.feedback.toasts$;
		this.confirm$ = this.feedback.confirm$;
	}

	dismissToast(id: number): void {
		this.feedback.dismissToast(id);
	}

	acceptConfirm(): void {
		this.feedback.respondConfirm(true);
	}

	rejectConfirm(): void {
		this.feedback.respondConfirm(false);
	}

	trackToast(_: number, toast: FeedbackToast): number {
		return toast.id;
	}
}