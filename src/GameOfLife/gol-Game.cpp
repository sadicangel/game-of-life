module gol;

constexpr char DEAD = '.';
constexpr char ALIVE = '*';

namespace gol
{
    Game::Game(const std::size_t size)
        :
        _size(size),
        _length(size * size),
        _main(new char[size * size]),
        _temp(new char[size * size]),
        _error("")
    {
        std::memset(_main.get(), DEAD, _length);
        std::memset(_temp.get(), DEAD, _length);
    }

    void Game::error(char* out, const std::size_t length) const
    {
        std::memcpy(out, _error.data(), std::min(length, _error.length()));
    }

    std::size_t Game::seed(const char* seed, const std::size_t length)
    {
        if (length != _length) {
            _error = "Expected seed of length to be '" + std::to_string(_length) + "'. Got '" + std::to_string(length) + "'";
            return _error.length();
        }

        std::memcpy(_main.get(), seed, length);

        return 0;
    }

    const char* Game::state() const
    {
        return _main.get();
    }

    const char Game::get_next_state(std::size_t i) const
    {
        const auto ul = i - _size - 1;
        const auto up = i - _size;
        const auto ur = i - _size + 1;
        const auto le = i - 1;
        const auto ri = i + 1;
        const auto dl = i + _size - 1;
        const auto dp = i + _size;
        const auto dr = i + _size + 1;

        size_t living = 0;
        const auto main = _main.get();
        if (ul >= 0 && ul < _length && main[ul] == ALIVE)
            living++;
        if (up >= 0 && up < _length && main[up] == ALIVE)
            living++;
        if (ur >= 0 && ur < _length && main[ur] == ALIVE)
            living++;
        if (le >= 0 && le < _length && main[le] == ALIVE)
            living++;
        if (ri >= 0 && ri < _length && main[ri] == ALIVE)
            living++;
        if (dl >= 0 && dl < _length && main[dl] == ALIVE)
            living++;
        if (dp >= 0 && dp < _length && main[dp] == ALIVE)
            living++;
        if (dr >= 0 && dr < _length && main[dr] == ALIVE)
            living++;

        const auto cell = main[i];
        if (cell == ALIVE) {
            return living == 2 || living == 3 ? ALIVE : DEAD;
        }
        else {
            return living == 3 ? ALIVE : DEAD;
        }
    }

    void Game::tick()
    {
        // TODO: Make this run in parallel?
        auto temp = _temp.get();
        for (size_t y = 0; y < _size; ++y) {
            for (size_t x = 0; x < _size; ++x) {
                const auto i = y * _size + x;
                temp[i] = get_next_state(i);
            }
        }
        std::swap(_main, _temp);
    }
}