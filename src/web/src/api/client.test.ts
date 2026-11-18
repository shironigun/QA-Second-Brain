import { apiGet, ApiError } from './client';

describe('api client', () => {
  afterEach(() => vi.restoreAllMocks());

  it('returns parsed JSON and forwards the AbortSignal', async () => {
    const fetchMock = vi
      .spyOn(globalThis, 'fetch')
      .mockResolvedValue(new Response(JSON.stringify({ status: 'ok' }), { status: 200 }));
    const controller = new AbortController();

    const body = await apiGet<{ status: string }>('/health', controller.signal);

    expect(body.status).toBe('ok');
    expect(fetchMock.mock.calls[0][1]?.signal).toBe(controller.signal);
  });

  it('throws ApiError with the status on a non-2xx response', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response('{}', { status: 503 }));
    await expect(apiGet('/health')).rejects.toMatchObject({ status: 503 } satisfies Partial<ApiError>);
  });
});
