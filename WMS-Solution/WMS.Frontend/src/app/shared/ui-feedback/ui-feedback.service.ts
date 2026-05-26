import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscriber } from 'rxjs';

export type FeedbackTone = 'success' | 'error' | 'info' | 'warning';

export interface FeedbackToast {
	id: number;
	tone: FeedbackTone;
	title: string;
	message: string;
	timeout: number;
}

export interface FeedbackConfirmOptions {
	title: string;
	message: string;
	confirmLabel?: string;
	cancelLabel?: string;
	tone?: FeedbackTone;
}

export interface FeedbackConfirmState {
	title: string;
	message: string;
	confirmLabel: string;
	cancelLabel: string;
	tone: FeedbackTone;
	subscriber: Subscriber<boolean>;
}

@Injectable({ providedIn: 'root' })
export class UiFeedbackService {
	private nextToastId = 1;
	private readonly toastTimers = new Map<number, ReturnType<typeof setTimeout>>();
	private readonly toastsSubject = new BehaviorSubject<FeedbackToast[]>([]);
	private readonly confirmSubject = new BehaviorSubject<FeedbackConfirmState | null>(null);
	private currentConfirm: FeedbackConfirmState | null = null;

	readonly toasts$ = this.toastsSubject.asObservable();
	readonly confirm$ = this.confirmSubject.asObservable();

	toast(tone: FeedbackTone, title: string, message: string, timeout = 4200): void {
		const toast: FeedbackToast = {
			id: this.nextToastId++,
			tone,
			title,
			message,
			timeout,
		};

		this.toastsSubject.next([...this.toastsSubject.value, toast]);

		const timer = setTimeout(() => this.dismissToast(toast.id), timeout);
		this.toastTimers.set(toast.id, timer);
	}

	success(title: string, message: string, timeout = 4200): void {
		this.toast('success', title, message, timeout);
	}

	error(title: string, message: string, timeout = 5200): void {
		this.toast('error', title, message, timeout);
	}

	info(title: string, message: string, timeout = 4200): void {
		this.toast('info', title, message, timeout);
	}

	warning(title: string, message: string, timeout = 4200): void {
		this.toast('warning', title, message, timeout);
	}

	confirm(options: FeedbackConfirmOptions): Observable<boolean> {
		return new Observable<boolean>((subscriber) => {
			const state: FeedbackConfirmState = {
				title: options.title,
				message: options.message,
				confirmLabel: options.confirmLabel ?? 'Confirm',
				cancelLabel: options.cancelLabel ?? 'Cancel',
				tone: options.tone ?? 'warning',
				subscriber,
			};

			this.currentConfirm = state;
			this.confirmSubject.next(state);

			return () => {
				if (this.currentConfirm?.subscriber === subscriber) {
					this.clearConfirm();
				}
			};
		});
	}

	respondConfirm(accepted: boolean): void {
		if (!this.currentConfirm) {
			return;
		}

		const current = this.currentConfirm;
		this.clearConfirm();
		current.subscriber.next(accepted);
		current.subscriber.complete();
	}

	dismissToast(id: number): void {
		const timer = this.toastTimers.get(id);

		if (timer) {
			clearTimeout(timer);
			this.toastTimers.delete(id);
		}

		this.toastsSubject.next(this.toastsSubject.value.filter((toast) => toast.id !== id));
	}

	private clearConfirm(): void {
		this.currentConfirm = null;
		this.confirmSubject.next(null);
	}
}