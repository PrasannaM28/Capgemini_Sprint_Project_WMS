import { AuthGuard } from './auth-guard';
import { Auth } from '../services/auth';
import { Router } from '@angular/router';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let authService: { hasToken: () => boolean };
  let router: { navigate: (...commands: string[][]) => void };

  beforeEach(() => {
    authService = {
      hasToken: () => false,
    };

    router = {
      navigate: () => undefined,
    };

    guard = new AuthGuard(
      authService as Auth,
      router as Router
    );
  });

  it('should allow navigation when a token exists', () => {
    authService.hasToken = () => true;

    expect(guard.canActivate()).toBe(true);
  });

  it('should redirect to login when no token exists', () => {
    let navigatedTo: string[][] = [];

    router = {
      navigate: (...commands: string[][]) => {
        navigatedTo = commands;
      },
    };

    guard = new AuthGuard(
      authService as Auth,
      router as Router
    );

    authService.hasToken = () => false;

    expect(guard.canActivate()).toBe(false);
    expect(navigatedTo).toEqual([['/login']]);
  });
});
