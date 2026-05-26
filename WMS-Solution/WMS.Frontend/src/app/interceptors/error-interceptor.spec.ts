import { ErrorInterceptor } from './error-interceptor';
import { UiFeedbackService } from '../shared/ui-feedback/ui-feedback.service';

describe('ErrorInterceptor', () => {
  const feedbackSpy = {
    error: () => undefined,
  } as unknown as UiFeedbackService;

  it('should be created', () => {
    expect(new ErrorInterceptor(feedbackSpy)).toBeTruthy();
  });
});
